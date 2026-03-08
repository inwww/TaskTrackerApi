# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY TaskTrackerApi.csproj .

# Restore dependencies
RUN dotnet restore "TaskTrackerApi.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "TaskTrackerApi.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "TaskTrackerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080
EXPOSE 8443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_HTTP_PORT=8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "TaskTrackerApi.dll"]
