# SortFlow

> Real-time logistics sorting dashboard with live metrics, exception tracking, and JWT authentication.

[![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)](https://react.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?logo=postgresql)](https://www.postgresql.org/)

## Features

- **Live dashboard** — Items/min, items/hour, total/successful/exceptions in the last hour
- **Exception list** — Type, item, station, details, and time (UTC)
- **Real-time updates** — SignalR pushes new sorting events to the dashboard
- **JWT auth** — Dev token endpoint for local sign-in; frontend stores token and calls protected APIs
- **Demo data** — Background service generates simulated sorting events and exceptions
- **Clean Architecture** — API, Application, Domain, Infrastructure (backend); React + Vite (frontend)

## Tech Stack

| Layer    | Technologies |
|----------|--------------|
| **Backend**  | .NET 8, Entity Framework Core, PostgreSQL, JWT, SignalR, Swagger, Serilog |
| **Frontend** | React 18, Vite, TypeScript, React Router, @microsoft/signalr |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node 18+](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/) (e.g. `localhost:5432`)

### Run the app

1. **PostgreSQL** — Ensure it’s running and a database exists (or use the connection string in `appsettings.json`; the app creates the DB on first run).

2. **API**
   ```bash
   cd sortflow-api/src/SortFlow.Api
   dotnet run
   ```
   → [http://localhost:5000](http://localhost:5000) · [Swagger](http://localhost:5000/swagger)

3. **Web**
   ```bash
   cd sortflow-web
   npm install
   npm run dev
   ```
   On Windows, if `npm` fails in PowerShell, use `install-and-dev.cmd` or run in Command Prompt.

   → [http://localhost:3000](http://localhost:3000)

4. **Use the app** — Open http://localhost:3000 → **Get dev token & sign in** → **Dashboard** (live) and **Exceptions**.

## Project Structure

| Folder           | Description |
|------------------|-------------|
| **sortflow-api** | .NET 8 Web API: Clean Architecture, EF Core + PostgreSQL, JWT, SignalR, background event generator |
| **sortflow-web** | React (Vite, TypeScript): Login, Dashboard, Exceptions, SignalR client |

More detail: [sortflow-api/README.md](sortflow-api/README.md) · [sortflow-web/README.md](sortflow-web/README.md)

## API

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/auth/token` | Dev JWT (Development only) |
| `GET`  | `/api/dashboard/summary` | Dashboard KPIs (last hour) |
| `GET`  | `/api/exceptions?limit=` | Recent exceptions |
| `GET`  | `/health` | Health check |
| `GET`  | `/swagger` | Swagger UI |
| SignalR | `/hubs/dashboard` | Hub; event `sortingEventReceived` |

## Configuration

- **API:** `sortflow-api/src/SortFlow.Api/appsettings.json`  
  - `ConnectionStrings:SortFlowDb` — PostgreSQL  
  - `Jwt:Key`, `Issuer`, `Audience`  
  - `Cors:AllowedOrigins` — e.g. `http://localhost:3000`

- **Web:** `sortflow-web/src/api/client.ts` — `API_BASE` (default `http://localhost:5000`)

## Security

`appsettings.json` includes a DB password and a dev JWT key. For production or a **public** repo, use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables and avoid committing secrets.
