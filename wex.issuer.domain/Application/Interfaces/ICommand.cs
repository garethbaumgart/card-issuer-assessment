using MediatR;

namespace wex.issuer.domain.Application.Interfaces;

/// <summary>
/// Marker interface for commands that return a result
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the command</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands that don't return a result
/// </summary>
public interface ICommand : IRequest
{
}