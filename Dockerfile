# Multi-stage Dockerfile for LendingPlatform.Api (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project files
COPY backend/LendingPlatform.slnx ./backend/
COPY backend/LendingPlatform.Domain/LendingPlatform.Domain.csproj ./backend/LendingPlatform.Domain/
COPY backend/LendingPlatform.Application/LendingPlatform.Application.csproj ./backend/LendingPlatform.Application/
COPY backend/LendingPlatform.Infrastructure/LendingPlatform.Infrastructure.csproj ./backend/LendingPlatform.Infrastructure/
COPY backend/LendingPlatform.Api/LendingPlatform.Api.csproj ./backend/LendingPlatform.Api/
COPY backend/LendingPlatform.Tests/LendingPlatform.Tests.csproj ./backend/LendingPlatform.Tests/

# Restore dependencies
RUN dotnet restore ./backend/LendingPlatform.Api/LendingPlatform.Api.csproj

# Copy all source files
COPY backend/ ./backend/

# Build and publish release output
WORKDIR /src/backend/LendingPlatform.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LendingPlatform.Api.dll"]
