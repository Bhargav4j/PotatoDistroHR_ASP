# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy solution and project files for dependency caching
COPY *.sln ./
COPY src/PotatoDistroHR.Web/*.csproj ./src/PotatoDistroHR.Web/
COPY src/PotatoDistroHR.Application/*.csproj ./src/PotatoDistroHR.Application/
COPY src/PotatoDistroHR.Domain/*.csproj ./src/PotatoDistroHR.Domain/
COPY src/PotatoDistroHR.Infrastructure/*.csproj ./src/PotatoDistroHR.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/src/PotatoDistroHR.Web
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from builder stage
COPY --from=builder /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose application port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "PotatoDistroHR.Web.dll"]