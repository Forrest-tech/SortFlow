# SortFlow

> Enterprise-ready logistics sorting factory platform: real-time dashboard, events, exceptions, history, zones, stations, configurable generator, and JWT auth.

[![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)](https://dotnet.microsoft.com/) [![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)](https://react.dev/) [![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)](https://www.postgresql.org/)

## Features

- **Dashboard** — Items/min, items/hour, total today; total events and circular success rate; events by category (Successful, AddressMismatch, DamagedLabel, InvalidPostalCode) and exceptions by category; configurable time window; SignalR live updates
- **Events / Exceptions** — Paged (configurable per-page: 10–200), filterable, sortable, resizable columns; summary bars; inner-table scroll with sticky header
- **History** — Events and Exceptions tabs; raw data by date range; day/week/month filter; CSV export; paged with per-page selector
- **Zones / Stations** — Full CRUD; business rules (e.g. no zone delete with stations)
- **Settings** — Generator rate, exception probabilities, dashboard window; affects runtime
- **Generator** — Background service; start/stop/status via `POST/GET /api/admin/generator/*`
- **Auth** — `POST /api/auth/login` (JWT); `POST /api/auth/token` (dev)

## Tech Stack

| Layer     | Technologies |
|-----------|--------------|
| **Backend**  | .NET 8, EF Core 8, Npgsql.EntityFrameworkCore.PostgreSQL 8, JWT, SignalR, Swagger, Serilog, Clean Architecture (Api / Application / Domain / Infrastructure), Migrations |
| **Frontend** | React 18, Vite 5, TypeScript 5.6, React Router 6, @microsoft/signalr 8 |

## Run with Docker (recommended)

```bash
cd sortflow-api
docker compose up -d
```

- **API:** http://localhost:5000 (Swagger: /swagger)
- **PostgreSQL:** `localhost:5432`, database `sortflow`, user `sortflow`, password `sortflow_pw`

Then run the web app:

```bash
cd sortflow-web
npm install && npm run dev
```

→ http://localhost:3000 — Sign in (e.g. dev/dev or **Get dev token**).

## Run locally (without Docker)

1. **PostgreSQL** — Running at `localhost:5432`. Create database `sortflow` and user/password (e.g. `sortflow` / `sortflow_pw`). Set `ConnectionStrings:SortFlowDb` in `appsettings.json` or use env `SortFlowDb`. EF DesignTimeDbContextFactory uses `SortFlowDb` or `Host=localhost;Port=5432;Database=sortflow;Username=sortflow;Password=sortflow_pw`.

2. **API**
   ```bash
   cd sortflow-api
   dotnet run --project src/SortFlow.Api
   ```
   Migrations and seed (Zones, Stations, AppSettings) run on startup.

3. **Web**
   ```bash
   cd sortflow-web
   npm install && npm run dev
   ```

## Project structure

| Folder           | Description |
|------------------|-------------|
| **sortflow-api** | .NET 8 solution: **SortFlow.Api** (Controllers, Hubs, Middleware, Services), **SortFlow.Application** (Abstractions, Services, Models), **SortFlow.Domain** (Entities, Enums), **SortFlow.Infrastructure** (DbContext, Repositories, Migrations, DataSeeder, DesignTimeDbContextFactory). Dockerfile, docker-compose. |
| **sortflow-web** | Vite + React: `src/pages` (Dashboard, Events, Exceptions, History, Zones, Stations, Settings, Login), `src/components`, `src/hooks`, `src/api`. |

## API overview

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/auth/login` | JWT (body: `username`, `password`) |
| `POST` | `/api/auth/token` | Dev JWT (Development only) |
| `GET`  | `/api/dashboard/summary` | `?windowMinutes=`, `timeFrom`, `timeTo`; returns `eventsByCategory` (OK, InvalidPostalCode, DamagedLabel, AddressMismatch) |
| `GET`  | `/api/events` | `page`, `pageSize` (1–200), `sortBy`, `sortDir`, `zoneId`, `stationId`, `timeFrom`, `timeTo`, `exceptionType`, `result` |
| `GET`  | `/api/exceptions` | Same query params as events (no `result`) |
| `GET`  | `/api/history` | `?groupBy=day|week|month`, `from`, `to` (aggregated) |
| `GET`  | `/api/history/export` | CSV; `from`, `to` |
| `GET/PUT` | `/api/settings` | AppSettings |
| `GET/POST/PUT/DELETE` | `/api/zones` | CRUD |
| `GET/POST/PUT/DELETE` | `/api/stations` | CRUD |
| `POST` | `/api/admin/generator/start` | Start generator |
| `POST` | `/api/admin/generator/stop` | Stop generator |
| `GET`  | `/api/admin/generator/status` | `isRunning`, `ratePerSecond` |
| `GET`  | `/health` | Health check |
| SignalR | `/hubs/dashboard` | `sortingEventReceived`, `dashboard:summaryUpdated`, `events:newBatch`, `exceptions:newBatch` |

## Configuration

- **API:** `sortflow-api/src/SortFlow.Api/appsettings.json`
  - `ConnectionStrings:SortFlowDb`
  - `Jwt:Key` (≥32 chars), `Issuer`, `Audience`
  - `Cors:AllowedOrigins`
  - `Serilog` (e.g. MinimumLevel, WriteTo)

- **Web:** `sortflow-web/src/api/client.ts` — `API_BASE` from `VITE_API_BASE` or default `http://localhost:5000`.

## Deploy to Azure

1. Create an **Azure App Service** (e.g. Linux, .NET 8).
2. Add **Application settings** (or Key Vault):
   - `ConnectionStrings__SortFlowDb` — Azure PostgreSQL or managed instance connection string
   - `Jwt__Key` — ≥32-character secret
   - `Cors__AllowedOrigins` — e.g. `https://yourfrontend.azurestaticapps.net`
3. Run **migrations** on first deploy (startup runs `Migrate()` and seed, or run `dotnet ef database update` from the API project against the production DB).
4. **GitHub Actions:** CD workflow (`cd.yml`) publishes `sortflow-api/src/SortFlow.Api/SortFlow.Api.csproj` and deploys to Azure. Configure secrets: `AZURE_WEBAPP_NAME`, `AZURE_WEBAPP_PUBLISH_PROFILE` (and optionally `AZURE_CREDENTIALS` for `azure/login@v2`).

## CI / CD

- **CI** (`ci.yml`): on push/PR to `main` — `dotnet restore/build/test` for `sortflow-api/SortFlow.sln`, `npm ci` and `npm run build` for `sortflow-web`.
- **CD** (`cd.yml`): on push to `main` — build and deploy API to Azure App Service.

## Security

Use **User Secrets** or environment variables for production; do not commit `Jwt:Key` or DB passwords.
