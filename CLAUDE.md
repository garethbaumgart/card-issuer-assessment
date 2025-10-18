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

## Current Implementation Status

✅ **Complete**: Production-ready card authorization system implemented
✅ **Framework**: .NET 9.0 with Entity Framework Core and native OpenAPI
✅ **Containerized**: Full Docker setup with automatic migrations
✅ **Tested**: 27 comprehensive unit tests covering domain boundaries

## Project Structure

The solution is implemented with Domain-Driven Design patterns:

```
wex.issuer.sln                    # Solution file
├── wex.issuer.api/               # Web API layer with controllers
│   └── Dockerfile                # API container with tests
├── wex.issuer.domain/            # Domain logic, entities, repositories  
├── wex.issuer.domain.tests/      # Unit tests (27 tests)
├── wex.issuer.migrations/        # EF Core database migrations
│   └── Dockerfile                # Self-contained migrations
└── docker-compose.yml            # Complete orchestration
```

## Running the Application

### Quick Start (Recommended)
```bash
# Start everything with one command
docker compose up

# Available at:
# - API: http://localhost:5001
# - Scalar UI: http://localhost:5001/scalar/v1
# - OpenAPI: http://localhost:5001/openapi/v1.json
```

### Development Mode
```bash
# Run tests
dotnet test wex.issuer.domain.tests

# Run API locally (requires Docker for DB)
docker compose up postgres -d
dotnet run --project wex.issuer.api
```

## Implementation Features

### Completed Requirements
- ✅ **Card Management**: Create and store cards with credit limits
- ✅ **Domain Validation**: Business rules enforcement with proper exceptions
- ✅ **Repository Pattern**: Clean data access abstraction
- ✅ **Unit Testing**: 27 tests covering Card and CardService boundaries
- ✅ **Production Setup**: Docker, migrations, health checks

### Architecture Highlights
- **Domain-Driven Design** with clean separation
- **Factory Pattern** for Card creation with validation
- **Unit of Work** pattern for transaction management
- **Immutable Entities** with encapsulated business logic
- **Automated Testing** integrated into build pipeline
- **Self-contained Deployment** via Docker Compose

### Key Business Rules Implemented
- Credit limits must be positive amounts > 0
- All amounts rounded to nearest cent (USD)
- Currency codes must be valid 3-character ISO codes
- Cards have unique identifiers and creation timestamps
- Comprehensive input validation and domain exceptions