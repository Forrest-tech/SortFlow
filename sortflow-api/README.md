# SortFlow API

Production-style .NET 8 Web API backend for the SortFlow logistics sorting platform.

## Features
- Clean Architecture (API, Application, Domain, Infrastructure)
- PostgreSQL with Entity Framework Core
- JWT Authentication
- Swagger/OpenAPI
- Health checks
- SignalR for real-time dashboard updates
- Background service to generate simulated sorting events
- Structured logging with Serilog
- CORS configured for React frontend

## Project Structure
```
src/
  SortFlow.Api/
  SortFlow.Application/
  SortFlow.Domain/
  SortFlow.Infrastructure/
```

## Local Development
### Prerequisites
- .NET 8 SDK
- Docker (for local Postgres)

### 1) Start Postgres
```
docker compose up -d
```

### 2) Run the API
```
dotnet run --project src/SortFlow.Api
```

### 3) Swagger
Navigate to:
```
https://localhost:5001/swagger
```

## Configuration
Update `src/SortFlow.Api/appsettings.json` for:
- Connection string
- JWT key/issuer/audience
- CORS allowed origins

## Endpoints
- `GET /api/dashboard/summary` - dashboard KPI summary
- `GET /api/exceptions` - recent exceptions
- `GET /health` - health check
- SignalR hub: `/hubs/dashboard`

## Notes
- The database is created automatically on startup.
- JWT authentication is enabled; obtain or mint a JWT in your auth provider.
