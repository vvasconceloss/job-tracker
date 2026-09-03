<div align="center">

# Job Tracker

> A lightweight job application tracking API, built as a deliberate ASP.NET Core + EF Core +
> PostgreSQL learning project — no shortcuts on architecture, validation or HTTP semantics.

<p align="center">

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10-512bd4.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14-239120.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791.svg)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10-8a2be2.svg)](https://learn.microsoft.com/en-us/ef/core/)
[![Status](https://img.shields.io/badge/status-MVP%20complete-brightgreen.svg)](docs/MVP.md)

</p>
</div>

---

## What is Job Tracker?

Job Tracker is a small ASP.NET Core Web API for keeping track of job applications — which companies I've applied to, which position, at what stage, and what the numbers look like overall. It replaces the usual mess of spreadsheets, bookmarks and browser tabs during a job search with a single source of truth, queryable through a dashboard endpoint.

It's built as a learning-first portfolio piece, backend-only (Scalar as the interface, no frontend): ASP.NET Core Web API, Entity Framework Core against PostgreSQL, a Controller → Service → DbContext layered architecture, DTOs, validation, centralized exception handling, and unit tests on the layer that actually contains business logic.

---

## Project Status

✅ MVP complete (Phases 0–6 in `docs/PLANNING.md`). No live deployment: this project is intentionally backend-only with Scalar as the interface, and hosting is out of scope for now.

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

A company has many applications. No `User`, no `Note` as a separate entity — deliberately small.

---

## Architecture

```
Client (Scalar / HTTP)
   │  HTTP
   ▼
ASP.NET Core Middleware   (routing, exception handling)
   │
   ▼
Controller
   │  binds request → DTO, validates
   ▼
Service        (ICompanyService, IJobApplicationService, IDashboardService — the only layer that talks to EF Core)
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
├── Controllers/          # CompaniesController, JobApplicationsController, DashboardController, BaseController
├── Data/                 # ApplicationDbContext, Configurations/
├── DTOs/                 # Company/, JobApplication/, Dashboard/
├── Enums/                # ApplicationStatus, ErrorType
├── Interfaces/           # ICompanyService, IJobApplicationService, IDashboardService
├── Services/             # CompanyService, JobApplicationService, DashboardService
├── Middlewares/          # ExceptionHandlingMiddleware
├── Common/               # Result, Error
├── Migrations/           # EF Core migrations
│
├── JobTracker.Tests/     # xUnit — Service-layer business rules (InMemory)
│   ├── Helpers/          # TestDbContextFactory
│   └── Services/         # JobApplicationServiceTests, DashboardServiceTests
│
├── Program.cs
├── appsettings.json
├── docker-compose.yml
└── JobTracker.http
```

No Repository Pattern, no Clean Architecture, no CQRS/MediatR. EF Core already abstracts data access, and this project's size doesn't justify the extra layers.

---

## API Overview

Base path: `/api`

### Companies

| Method | Route             | Description    | Success | Errors   |
| ------ | ----------------- | -------------- | ------- | -------- |
| POST   | `/companies`      | Create company | 201     | 400, 409 |
| GET    | `/companies`      | List companies | 200     | —        |
| GET    | `/companies/{id}` | Get company    | 200     | 404      |

### Job Applications

| Method | Route                       | Description         | Success | Errors             |
| ------ | --------------------------- | ------------------- | ------- | ------------------ |
| POST   | `/applications`             | Create application  | 201     | 400, 404 (company) |
| GET    | `/applications`             | List (with filters) | 200     | —                  |
| GET    | `/applications/{id}`        | Get application     | 200     | 404                |
| PUT    | `/applications/{id}`        | Update data         | 200     | 400, 404           |
| PATCH  | `/applications/{id}/status` | Change status       | 200     | 400, 404           |
| DELETE | `/applications/{id}`        | Delete              | 204     | 404                |

**Supported filters on `GET /applications` (combinable with AND):**

```
?status=Interview
?companyId=1
?from=2026-08-01&to=2026-08-31
```

### Dashboard

| Method | Route           | Description                                  | Success |
| ------ | --------------- | -------------------------------------------- | ------- |
| GET    | `/dashboard`    | Totals, rates and aggregations by status/company | 200     |

Example `GET /api/dashboard` response:

```json
{
  "totalApplications": 25,
  "applicationsThisMonth": 12,
  "interviews": 4,
  "technicalTests": 2,
  "offers": 1,
  "rejections": 8,
  "withdrawn": 1,
  "interviewRate": 16.0,
  "applicationsByStatus": {
    "Applied": 8,
    "Screening": 3,
    "Interview": 4
  },
  "applicationsByCompany": [{ "company": "Integer Consulting", "count": 5 }]
}
```

**Error contract (standard, via middleware):**

```json
{
  "status": 404,
  "message": "Job application not found."
}
```

---

## Getting Started (development)

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 16 (local install or Docker)
- Docker & Docker Compose (optional, for local Postgres)

### Setup

```bash
git clone https://github.com/vvasconceloss/job-tracker.git
cd job-tracker
```

Create `.env` from example (used by `docker-compose.yml`):

```bash
# .env
DB_NAME=jobtracker
DB_USER=jobtracker_admin
DB_PASSWORD=your_secure_password
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
  "Host=localhost;Port=5433;Database=jobtracker;Username=jobtracker_admin;Password=your_secure_password"
```

Or set via `appsettings.Development.json` (kept out of git):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=5433;Database=jobtracker;Username=jobtracker_admin;Password=your_secure_password"
  }
}
```

Apply migrations:

```bash
dotnet ef database update
```

Run the app:

```bash
dotnet run
```

The API will be available at `http://localhost:5099` (and `https://localhost:7115` via `launchSettings.json`), with Scalar UI at `/scalar/v1` in Development.

### Populate with real data

Use Scalar (`/scalar/v1`) to run the flow from `docs/MVP.md §2`:

```
Create company → Create application → Query → Filter → Change status → Update → Delete → View dashboard
```

Example requests are in `JobTracker.http` (VS Code / Rider HTTP client). For bulk seed, create companies and applications via `POST /api/companies` and `POST /api/applications` — see `docs/MVP.md §4` for the full contract.

---

## Testing

```bash
dotnet test JobTracker.Tests/JobTracker.Tests.csproj
# or
dotnet test
```

Unit tests cover the Service layer, where the actual business rules live — not trivial CRUD or controllers:

- `Should_Create_Application`
- `Should_Reject_Unknown_Company`
- `Should_Reject_Negative_Or_Inverted_Salary`
- `Should_Reject_Invalid_Status_Transition` (terminal states: Offer/Rejected/Withdrawn)
- `Should_Calculate_Interview_Rate` (incl. empty DB and rounding edge cases)

7 tests, 0 failures.

---

## Screenshots

> Scalar UI — add screenshots to `docs/images/` and reference them here for the portfolio.

- `docs/images/scalar-dashboard.png` — `GET /api/dashboard` response
- `docs/images/scalar-create-application.png` — `POST /api/applications`
- `docs/images/scalar-filter.png` — `GET /api/applications?status=Interview`

---

## Stack

```
Backend      C# 14, ASP.NET Core 10 Web API, EF Core 10, DI, DTOs, Validation, Exception Handling
Database     PostgreSQL 16, relational modeling, FKs, GroupBy/Count aggregations, migrations
Engineering  Layered architecture, business logic, HTTP semantics (201/200/204/400/404/409), xUnit, Scalar, Git
```

See `docs/MVP.md` for the MVP scope and `docs/PLANNING.md` for build phases and definition of done.

---

## License

This project is licensed under the MIT License.
