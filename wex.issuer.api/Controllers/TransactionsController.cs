using MediatR;
using Microsoft.AspNetCore.Mvc;
using wex.issuer.api.DTOs;
using wex.issuer.domain.Application.Commands;
using wex.issuer.domain.Application.Queries;
using wex.issuer.domain.Exceptions;

namespace wex.issuer.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    : ControllerBase
{
    /// <summary>
    /// Creates a new transaction for a card
    /// </summary>
    /// <param name="request">The transaction creation request</param>
    /// <returns>The created transaction details</returns>
    /// <response code="201">Transaction created successfully</response>
    /// <response code="400">Invalid request data or domain validation failure</response>
    /// <response code="404">Card not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TransactionResponse>> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create command from request
            var command = new CreateTransactionCommand(
                request.CardId,
                request.Description,
                request.TransactionDate,
                request.PurchaseAmount
            );

            // Execute through transaction service
            var transaction = await mediator.Send(command);

            // Map to response DTO
            var response = new TransactionResponse
            {
                Id = transaction.Id,
                CardId = transaction.CardId,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                PurchaseAmount = transaction.PurchaseAmount,
                CreatedAt = transaction.CreatedAt
            };

            logger.LogInformation("Transaction created successfully with ID: {TransactionId} for Card: {CardId}", 
                transaction.Id, transaction.CardId);

            // Return 201 Created with location header
            return CreatedAtAction(
                nameof(GetTransaction),
                new { id = transaction.Id },
                response
            );
        }
        catch (DomainException ex) when (ex.Message.Contains("not found"))
        {
            logger.LogWarning("Card not found: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain validation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while creating transaction");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Retrieves a transaction by its unique identifier
    /// </summary>
    /// <param name="id">The transaction's unique identifier</param>
    /// <returns>The transaction details if found</returns>
    /// <response code="200">Transaction found and returned</response>
    /// <response code="404">Transaction not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponse>> GetTransaction(Guid id)
    {
        try
        {
            var query = new GetTransactionByIdQuery(id);
            var transaction = await mediator.Send(query);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            var response = new TransactionResponse
            {
                Id = transaction.Id,
                CardId = transaction.CardId,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                PurchaseAmount = transaction.PurchaseAmount,
                CreatedAt = transaction.CreatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving transaction {TransactionId}", id);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Retrieves all transactions for a specific card
    /// </summary>
    /// <param name="cardId">The card's unique identifier</param>
    /// <param name="skip">Number of transactions to skip (for pagination)</param>
    /// <param name="take">Number of transactions to take (for pagination, default 50, max 100)</param>
    /// <returns>List of transactions for the card</returns>
    /// <response code="200">Transactions found and returned</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet("card/{cardId}")]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByCard(
        Guid cardId, 
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 50)
    {
        try
        {
            // Validate pagination parameters
            if (skip < 0)
            {
                return BadRequest("Skip parameter must be non-negative");
            }

            if (take < 0)
            {
                return BadRequest("Take parameter must be non-negative");
            }

            var query = new GetTransactionsByCardIdQuery(cardId, skip, take);
            var transactions = await mediator.Send(query);

            var response = transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                CardId = t.CardId,
                Description = t.Description,
                TransactionDate = t.TransactionDate,
                PurchaseAmount = t.PurchaseAmount,
                CreatedAt = t.CreatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving transactions for card {CardId}", cardId);
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}