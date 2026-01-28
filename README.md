# SortFlow --- Cloud-Native Internal Operations Platform

SortFlow is a cloud-native internal operations platform designed to
support real-time monitoring, exception management, and operational
analytics for enterprise internal systems.

The platform is built using a modern full-stack architecture with
**React (TypeScript)**, **ASP.NET Core**, and **Microsoft Azure**, and
is deployed with automated CI/CD pipelines for continuous delivery.

This repository represents a **production-style reference
implementation** of an internal enterprise platform, including cloud
deployment, real-time updates, and scalable backend services.

------------------------------------------------------------------------

## Key Capabilities

-   Real-time operational dashboards\
-   Event ingestion and processing (50,000+ events/day scale)\
-   Exception tracking and categorization workflows\
-   Configurable system settings and business rules\
-   Historical reporting and operational analytics\
-   Architecture ready for role-based access and multi-user internal
    systems\
-   Automated CI/CD and cloud deployment

------------------------------------------------------------------------

## Architecture Overview

### Frontend

-   React + TypeScript\
-   Vite build system\
-   Real-time UI updates via SignalR\
-   Deployed to **Azure Static Web Apps**

### Backend

-   ASP.NET Core Web API\
-   Clean Architecture (API / Application / Domain / Infrastructure
    layers)\
-   RESTful APIs + SignalR\
-   Hosted on **Azure App Services**

### Data

-   Azure Database for PostgreSQL\
-   Entity Framework Core\
-   Operational and historical data modeling\
-   Centralized persistence for events, exceptions, and system
    configuration

### Cloud & DevOps

-   Microsoft Azure
    -   Azure Static Web Apps (Frontend)\
    -   Azure App Services (Backend APIs)\
    -   Azure Database for PostgreSQL\
-   GitHub Actions for CI/CD\
-   Automated build, test, and deployment pipelines\
-   Environment-based configuration and secrets management

------------------------------------------------------------------------

## Cloud Deployment & CI/CD

The platform is deployed to Microsoft Azure with fully automated CI/CD
pipelines.

Each commit to the main branch triggers:

-   Frontend build and deployment to Azure Static Web Apps\
-   Backend build and deployment to Azure App Services\
-   Automated validation and deployment through GitHub Actions

This ensures consistent, repeatable, and low-risk deployments across
environments.

------------------------------------------------------------------------

## Business Scenarios Supported

SortFlow is designed to support real-world internal operational use
cases, including:

-   Real-time monitoring of operational events\
-   Centralized exception tracking and categorization\
-   Operational trend analysis and historical reporting\
-   Configurable business rules and validation logic\
-   Internal user dashboards for operations and management teams

The platform architecture supports future extension for advanced
workflows, automation, and integrations with additional internal
systems.

------------------------------------------------------------------------

## Local Development

### Prerequisites

-   .NET 8 SDK\
-   Node.js (LTS)\
-   Docker Desktop

### Backend

``` bash
cd sortflow-api
docker compose up -d
dotnet run
```

### Frontend

``` bash
cd sortflow-web
npm install
npm run dev
```

------------------------------------------------------------------------

## Project Structure

    sortflow-api/
      - API layer (ASP.NET Core)
      - Application layer (business logic)
      - Domain layer (core domain models)
      - Infrastructure layer (data access, integrations)

    sortflow-web/
      - React + TypeScript frontend
      - Real-time dashboards and internal UI modules

    .github/workflows/
      - GitHub Actions CI/CD pipelines

------------------------------------------------------------------------

## CI/CD Pipelines

This repository uses GitHub Actions to:

-   Build and test backend services\
-   Build frontend assets\
-   Deploy backend APIs to Azure App Services\
-   Deploy frontend to Azure Static Web Apps

This setup enables continuous delivery and rapid iteration with minimal
manual intervention.

------------------------------------------------------------------------

## Screenshots

![Login](docs/screenshots/01-login.png)
![Dashboard](docs/screenshots/02-dashboard.png)
![Events](docs/screenshots/03-events.png)
![Exceptions](docs/screenshots/04-exceptions.png)
![History](docs/screenshots/05-history.png)
![Zones](docs/screenshots/06-zones.png)
![Stations](docs/screenshots/07-stations.png)
![Settings](docs/screenshots/08-settings.png)

------------------------------------------------------------------------

## Disclaimer

This repository contains a reference implementation and demonstration
environment for an internal enterprise operations platform. It is
intended to showcase full-stack architecture, cloud deployment, and
DevOps practices.
