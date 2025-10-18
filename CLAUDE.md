# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a WEX technical assessment project for implementing a card authorization system in C#. The solution requires building a production-ready application with card management, purchase transactions, currency conversion, and balance calculations.

## Technical Assessment Requirements

Based on the README.md, this application must implement:

1. **Card Management**: Create and store cards with credit limits
2. **Purchase Transactions**: Store purchase transactions associated with cards  
3. **Currency Conversion**: Retrieve transactions converted to specified currencies using Treasury API
4. **Balance Calculation**: Calculate available card balance in any supported currency

### Key Business Rules
- All amounts in USD, rounded to nearest cent
- Transaction descriptions limited to 50 characters
- Currency conversion uses Treasury Reporting Rates of Exchange API
- Exchange rates must be ≤ purchase date within last 6 months
- Available balance = credit limit - total purchases

## Architecture Requirements

- **Language**: C# (Mobility/Payments team requirement)
- **Framework**: .NET 9.0
- **Self-contained**: Must run without external databases/servers
- **Production-ready**: Include comprehensive functional automated testing
- **External API**: Integration with https://fiscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/treasury-reporting-rates-of-exchange

## Current Issues

✅ **Framework**: The project uses .NET 9.0 with proper support for all features including Entity Framework Core and native OpenAPI.

## Common Commands

## Current Project Structure

The solution has been implemented with Domain-Driven Design patterns:

```
wex.issuer.sln                    # Solution file
├── wex.issuer.api/               # Web API layer with controllers
├── wex.issuer.domain/            # Domain logic, entities, repositories  
├── wex.issuer.migrations/        # EF Core database migrations
├── docker-compose.yml            # PostgreSQL database container
└── DATABASE_SETUP.md             # Database setup instructions
```

### Build and Run
```bash
# Start database
docker compose up -d postgres

# Apply migrations
dotnet ef database update --project wex.issuer.migrations

# Run API
dotnet run --project wex.issuer.api

# Available at:
# - API: http://localhost:5000
# - Scalar UI: http://localhost:5000/scalar/v1
# - OpenAPI: http://localhost:5000/openapi/v1.json
```

### Testing
```bash
# Add test project (recommended: xunit)
dotnet new xunit -n wex.issuer.tests
dotnet sln add wex.issuer.tests

# Run tests
dotnet test
```

### Package Management
```bash
dotnet restore
dotnet add package [PackageName]
```

## Recommended Architecture

For a production-ready card authorization system:

- **Controllers/API Layer**: RESTful endpoints for card and transaction operations
- **Services Layer**: Business logic for card management, currency conversion
- **Repository Pattern**: Data persistence abstraction  
- **Models/DTOs**: Card, Transaction, Currency conversion entities
- **External Services**: Treasury API client for exchange rates
- **Validation**: Input validation and business rule enforcement
- **Logging**: Structured logging for production monitoring
- **Error Handling**: Comprehensive exception handling and user-friendly errors

## Development Focus Areas

Given the 4-hour time recommendation:
1. Core domain models (Card, Transaction)
2. Basic CRUD operations with in-memory storage
3. Treasury API integration for currency conversion
4. Essential business logic (balance calculation, validation)
5. Key unit tests for business logic
6. Simple API endpoints (minimal viable interface)