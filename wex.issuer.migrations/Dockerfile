# Use the official .NET 9.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set the working directory
WORKDIR /src

# Copy global.json first to ensure correct SDK version
COPY global.json ./

# Copy solution and project files
COPY wex.issuer.sln ./
COPY wex.issuer.api/wex.issuer.api.csproj ./wex.issuer.api/
COPY wex.issuer.domain/wex.issuer.domain.csproj ./wex.issuer.domain/
COPY wex.issuer.migrations/wex.issuer.migrations.csproj ./wex.issuer.migrations/
COPY wex.issuer.domain.tests/wex.issuer.domain.tests.csproj ./wex.issuer.domain.tests/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the application source code
COPY . ./

# Build the solution first
RUN dotnet build --no-restore -c Release

# Publish the migration project to ensure all dependencies are available
RUN dotnet publish wex.issuer.migrations/wex.issuer.migrations.csproj -c Release -o /app/migrations --no-restore

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Create migration script
RUN echo '#!/bin/bash\n\
echo "Waiting for database..."\n\
sleep 15\n\
echo "Running database migrations..."\n\
cd /src\n\
dotnet ef database update --project wex.issuer.migrations --configuration Release --verbose\n\
if [ $? -eq 0 ]; then\n\
    echo "Migrations completed successfully"\n\
else\n\
    echo "Migrations failed!"\n\
    exit 1\n\
fi' > /src/migrate.sh && chmod +x /src/migrate.sh

ENTRYPOINT ["/src/migrate.sh"]