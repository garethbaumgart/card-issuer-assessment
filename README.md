# WEX Card Issuer API

A production-ready card authorization system built with C# and .NET 9.0, implementing Domain-Driven Design patterns.

## Quick Start

### Prerequisites
- Docker and Docker Compose

### Running the Application

```bash
# Clone and navigate to the project
git clone https://github.com/garethbaumgart/card-issuer-assessment.git
Navigate to the root folder (same location as your docker-compose file)

# Start everything with a single command
docker compose up
```

The application will:
1. Start PostgreSQL database
2. Run database migrations automatically  
3. Execute all unit tests during build
4. Launch the API server

### Available Endpoints

- **API**: http://localhost:5001
- **API Documentation**: http://localhost:5001/scalar/v1
- **OpenAPI Spec**: http://localhost:5001/openapi/v1.json

### Example Usage

Create a new card:
```bash
curl -X POST http://localhost:5001/api/Cards \
  -H "Content-Type: application/json" \
  -d '{"creditLimit": 1000.00}'
```

## Technical Implementation

### Architecture
- **Domain-Driven Design** with clean separation of concerns
- **Repository Pattern** for data access abstraction
- **Unit of Work** for transaction management
- **Factory Pattern** for domain entity creation

### Project Structure
```
├── wex.issuer.api/              # Web API layer
├── wex.issuer.domain/           # Domain logic, entities, persistence and external services - can/should be broken up further as needed
├── wex.issuer.domain.tests/     # Unit tests (27 tests)
├── wex.issuer.migrations/       # EF Core database migrations
└── docker-compose.yml           # Container orchestration
```

### Features Implemented

✅ **Requirement #1**: Create and store cards with credit limits  
✅ **Database**: PostgreSQL with automatic migrations  
✅ **Production-Ready**: Comprehensive unit testing (27 tests)  
✅ **Containerized**: Zero-configuration deployment  
✅ **API Documentation**: Interactive Scalar UI  

### Quality Assurance
- **Unit Tests** covering core functionality
- **Automated Test Execution** in a Docker build pipeline

### Business Rules Enforced
- Credit limits must be positive amounts > 0
- All amounts rounded to nearest cent (USD)
- Currency codes must be valid 3-character ISO codes
- Cards have unique identifiers and creation timestamps

## Development

### Running Tests Locally
```bash
dotnet test wex.issuer.domain.tests
```

### Building Without Docker
```bash
dotnet restore
dotnet build
dotnet run --project wex.issuer.api
```

### Database Management
Database migrations run automatically in the containerized setup. The system uses PostgreSQL with Entity Framework Core for data persistence.

## Technology Stack

- **.NET 9.0** - Latest framework version
- **ASP.NET Core** - Web API framework  
- **Entity Framework Core** - ORM with PostgreSQL
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework for tests
- **Scalar** - Interactive API documentation
- **Docker** - Containerization platform

## Production Considerations

This application is built with production deployment in mind:

- **Health Checks**: Database connectivity validation
- **Logging**: Structured logging for monitoring
- **Error Handling**: Comprehensive exception management
- **Validation**: Input validation and domain rule enforcement
- **Security**: No sensitive data exposure in logs
- **Performance**: Optimized Docker images with multi-stage builds
