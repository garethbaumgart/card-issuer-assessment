FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

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

# Build the application
RUN dotnet build --no-restore -c Release

# Run unit tests
RUN dotnet test wex.issuer.domain.tests --no-build --configuration Release --logger trx --verbosity normal

# Publish the API project
RUN dotnet publish wex.issuer.api/wex.issuer.api.csproj -c Release -o /app/publish --no-restore

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port 8080 (ASP.NET Core default for containers)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
# ENV ASPNETCORE_ENVIRONMENT is set in the docker-compopse.yml

# Set the entrypoint to run the API
ENTRYPOINT ["dotnet", "wex.issuer.api.dll"]