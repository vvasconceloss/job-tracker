<div align="center">

# Job Tracker

> A lightweight job application tracking API, built as a deliberate ASP.NET Core + EF Core +
> PostgreSQL learning project — no shortcuts on architecture, validation or HTTP semantics.

<p align="center">

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8-512bd4.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791.svg)](https://www.postgresql.org/)
[![Status](https://img.shields.io/badge/status-in%20development-yellow.svg)](MVP.md)

</p>
</div>

---

## What is Job Application Tracker?

Job Application Tracker is a small ASP.NET Core Web API for keeping track of job applications —
which companies I've applied to, which position, at what stage, and what the numbers look like
overall. It replaces the usual mess of spreadsheets, bookmarks and browser tabs during a job search
with a single source of truth, queryable through a dashboard endpoint.

It's built as a learning-first portfolio piece, backend-only (Scalar as the interface, no
frontend): ASP.NET Core Web API, Entity Framework Core against PostgreSQL, a Controller → Service →
DbContext layered architecture, DTOs, validation, centralized exception handling, and unit tests on
the layer that actually contains business logic.

---

## Project Status

🚧 In development. No live deployment: this project is intentionally
backend-only with Scalar as the interface, and hosting is out of scope for now.

---

## Domain Model

```
Company
   │ 1
   │
   │ N
   ▼
JobApplication
```

---

## Architecture

```
Client (Swagger)
   │  HTTP
   ▼
ASP.NET Core Middleware   (routing, exception handling)
   │
   ▼
Controller
   │  binds request → DTO, validates
   ▼
Service        (ICompanyService, IJobApplicationService — the only layer that talks to EF Core)
   │
   ▼
EF Core  (ApplicationDbContext)
   │
   ▼
PostgreSQL
```

```
JobTracker/
│
├── Controllers/          # CompaniesController, JobApplicationsController
├── Data/                 # ApplicationDbContext, Configurations
├── Models/               # Company, JobApplication
├── DTOs/                 # Companies/, JobApplications/
├── Enums/                # ApplicationStatus
├── Services/             # Interfaces/, CompanyService, JobApplicationService
├── Middleware/           # ExceptionHandlingMiddleware
│
├── Program.cs
└── appsettings.json
tests/
└── JobTracker.Tests/      # xUnit — Service-layer business rules
```

No Repository Pattern, no Clean Architecture, no CQRS/MediatR. EF Core already
abstracts data access, and this project's size doesn't justify the extra layers.

---

## API Overview

| Method | Route                           | Description                                              |
| ------ | ------------------------------- | -------------------------------------------------------- |
| POST   | `/api/companies`                | Create company                                           |
| GET    | `/api/companies`                | List companies                                           |
| GET    | `/api/companies/{id}`           | Get company                                              |
| POST   | `/api/applications`             | Create application                                       |
| GET    | `/api/applications`             | List (filterable by `status`, `companyId`, `from`, `to`) |
| GET    | `/api/applications/{id}`        | Get application                                          |
| PUT    | `/api/applications/{id}`        | Update application                                       |
| PATCH  | `/api/applications/{id}/status` | Change status                                            |
| DELETE | `/api/applications/{id}`        | Delete application                                       |
| GET    | `/api/dashboard`                | Totals, rates and aggregations by status/company         |

---

## Getting Started (development)

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 16 (local install or Docker)

### Setup

```bash
git clone https://github.com/vvasconceloss/job-tracker.git
cd job-application-tracker
```

Start PostgreSQL (Docker):

```bash
docker compose up -d
```

Restore and build:

```bash
dotnet restore
dotnet build
```

Configure local secrets (outside version control):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=jobtracker;Username=jobtracker;Password=jobtracker" \
  --project src/JobTracker
```

Apply migrations:

```bash
dotnet ef database update --project src/JobTracker
```

Run the app:

```bash
dotnet run --project src/JobTracker
```

The API will be available at `http://localhost:5099`, with Scalar UI at `/scalar/v1` in development.

---

## Testing

```bash
dotnet test
```

Unit tests cover the Service layer, where the actual business rules live (status transitions,
validation, dashboard calculations) — not trivial CRUD or controllers.

---

## License

This project is licensed under the MIT License.
