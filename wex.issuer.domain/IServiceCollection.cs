using Microsoft.Extensions.DependencyInjection;
using wex.issuer.domain.External;
using wex.issuer.domain.Infrastructure.Repositories;
using wex.issuer.domain.Repositories;

namespace wex.issuer.domain;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all domain services including MediatR, repositories, and unit of work
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Entities.Card).Assembly));

        // Add Repository Pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Add External Services
        services.AddScoped<ITreasuryApiService, TreasuryApiService>();

        return services;
    }
}