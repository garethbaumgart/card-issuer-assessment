using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using wex.issuer.domain;
using wex.issuer.domain.External;
using wex.issuer.domain.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add Entity Framework
builder.Services.AddDbContext<WexIssuerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("wex.issuer.migrations")));

// Add HTTP Client for Treasury API
builder.Services.AddHttpClient<ITreasuryApiService, TreasuryApiService>();

builder.Services.AddDomainServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi(); // Enable OpenAPI for all environments
app.MapScalarApiReference(); // Enable Scalar UI

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();