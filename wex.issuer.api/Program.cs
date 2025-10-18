using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using wex.issuer.domain.Infrastructure;
using wex.issuer.domain.Infrastructure.Repositories;
using wex.issuer.domain.Repositories;
using wex.issuer.domain.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add Entity Framework
builder.Services.AddDbContext<WexIssuerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("wex.issuer.migrations")));

// Add Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICardRepository, CardRepository>();

// Add Application Services
builder.Services.AddScoped<CardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi(); // Enable OpenAPI for all environments
app.MapScalarApiReference(); // Enable Scalar UI

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();