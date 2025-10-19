FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src

# Copy only what's needed for migrations
COPY global.json ./
COPY wex.issuer.domain/ ./wex.issuer.domain/
COPY wex.issuer.migrations/ ./wex.issuer.migrations/

# Restore and install EF tools
RUN dotnet restore wex.issuer.migrations/ && \
    dotnet tool install --global dotnet-ef --version 9.*

ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "wex.issuer.migrations"]