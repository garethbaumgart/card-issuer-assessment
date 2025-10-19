using MediatR;

namespace wex.issuer.domain.Application.Interfaces;

/// <summary>
/// Marker interface for queries that return a result
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the query</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}