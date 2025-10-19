using MediatR;
using Microsoft.AspNetCore.Mvc;
using wex.issuer.api.DTOs;
using wex.issuer.domain.Application.Commands;
using wex.issuer.domain.Application.Queries;
using wex.issuer.domain.Exceptions;

namespace wex.issuer.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CardsController> _logger;

    public CardsController(IMediator mediator, ILogger<CardsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new card with the specified credit limit
    /// </summary>
    /// <param name="request">The card creation request</param>
    /// <returns>The created card details</returns>
    /// <response code="201">Card created successfully</response>
    /// <response code="400">Invalid request data or domain validation failure</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CardResponse>> CreateCard([FromBody] CreateCardRequest request)
    {
        try
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create command from request
            var command = new CreateCardCommand(
                request.CreditLimit,
                request.Currency ?? "USD"
            );

            // Execute through mediator
            var card = await _mediator.Send(command);

            // Map to response DTO
            var response = new CardResponse
            {
                Id = card.Id,
                CreditLimit = card.CreditLimit,
                Currency = card.Currency,
                CreatedAt = card.CreatedAt,
                AvailableBalance = card.GetAvailableBalance()
            };

            _logger.LogInformation("Card created successfully with ID: {CardId}", card.Id);

            // Return 201 Created with location header
            return CreatedAtAction(
                nameof(GetCard),
                new { id = card.Id },
                response
            );
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Domain validation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating card");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Retrieves a card by its unique identifier
    /// </summary>
    /// <param name="id">The card's unique identifier</param>
    /// <returns>The card details if found</returns>
    /// <response code="200">Card found and returned</response>
    /// <response code="404">Card not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardResponse>> GetCard(Guid id)
    {
        try
        {
            var query = new GetCardByIdQuery(id);
            var card = await _mediator.Send(query);

            if (card == null)
            {
                return NotFound($"Card with ID {id} not found");
            }

            var response = new CardResponse
            {
                Id = card.Id,
                CreditLimit = card.CreditLimit,
                Currency = card.Currency,
                CreatedAt = card.CreatedAt,
                AvailableBalance = card.GetAvailableBalance()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving card {CardId}", id);
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}