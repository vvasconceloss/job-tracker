# Job Tracker — Implementation Plan

Companion to `MVP.md`. This document covers **architecture, technical decisions, build phases, and definition of done** — the "how" and "in what order," not the "what."

## 1. Architecture

```
JobTracker/
│
├── Controllers/
│   ├── CompaniesController.cs
│   ├── JobApplicationsController.cs
│   ├── DashboardController.cs
│   └── BaseController.cs
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Configurations/
│       ├── CompanyConfiguration.cs
│       └── JobApplicationConfiguration.cs
│
├── DTOs/
│   ├── Company/
│   ├── JobApplication/
│   └── Dashboard/
│
├── Enums/
│   ├── ApplicationStatus.cs
│   └── ErrorType.cs
│
├── Interfaces/
│   ├── ICompanyService.cs
│   ├── IJobApplicationService.cs
│   └── IDashboardService.cs
│
├── Services/
│   ├── CompanyService.cs
│   ├── JobApplicationService.cs
│   └── DashboardService.cs
│
├── Middlewares/
│   └── ExceptionHandlingMiddleware.cs
│
├── Common/
│   ├── Result.cs
│   └── Error.cs
│
├── Models/
│   ├── Company.cs
│   └── JobApplication.cs
│
├── JobTracker.Tests/
│   ├── Helpers/TestDbContextFactory.cs
│   └── Services/
│
└── Program.cs
```

Dependency flow:

```
Controller → IService → Service → ApplicationDbContext → PostgreSQL
```

## 2. Build phases

Each phase should compile and run before moving to the next. Don't move to the next phase "halfway."

### Phase 0 — Setup

- [x] `dotnet new webapi` (or minimal API + controllers, per preference)
- [x] Set up local PostgreSQL (Docker or native install)
- [x] Install `Npgsql.EntityFrameworkCore.PostgreSQL`
- [x] Configure `ApplicationDbContext` and connection string in `appsettings.json` (+ `appsettings.Development.json` kept out of git)
- [x] First `dotnet ef migrations add InitialCreate` + `dotnet ef database update`

### Phase 1 — Company (complete vertical slice)

- [x] `Company` model
- [x] DTOs: `CreateCompanyDto`, `CompanyResponseDto`
- [x] `ICompanyService` / `CompanyService`
- [x] `CompanyController`: `POST`, `GET`, `GET /{id}`
- [x] Validation: name required and unique
- [x] Test everything in Swagger before moving on

Doing **Company first, on its own**, is intentional: it's the simplest slice of the system and it's used to solidify the Controller → Service → DbContext pattern before introducing the 1:N relationship.

### Phase 2 — JobApplication (core of the system)

- [x] `ApplicationStatus` enum
- [x] `JobApplication` model + relationship with `Company`
- [x] Migration for the new table + FK
- [x] DTOs: `CreateJobApplicationDto`, `UpdateJobApplicationDto`, `JobApplicationResponseDto`
- [x] `IJobApplicationService` / `JobApplicationService`
- [x] `JobApplicationsController`: `POST`, `GET`, `GET /{id}`, `PUT`, `DELETE`
- [x] Validations: `CompanyId` exists, `SalaryMax >= SalaryMin`, `AppliedAt` not in the future

### Phase 3 — Filters and status

- [x] `GET /applications` with combinable `status`, `companyId`, `from`, `to`
- [x] `PATCH /applications/{id}/status` with basic transition validation
- [x] Global exception middleware + standardized error contract

### Phase 4 — Dashboard

- [x] Totals query and aggregation by status
- [x] Aggregation by company
- [x] `interviewRate` calculation
- [x] `GET /dashboard`

This is the phase that trains real LINQ/EF Core the most (`GroupBy`, `Count`, combined `Where`s).

### Phase 5 — Essential tests

Only at the Service layer, where real business logic lives:

- [x] `Should_Create_Application`
- [x] `Should_Reject_Invalid_Status_Transition`
- [x] `Should_Reject_Unknown_Company`
- [x] `Should_Reject_Negative_Or_Inverted_Salary`
- [x] `Should_Calculate_Interview_Rate`

Don't test trivial CRUD or controllers — the return on that time is low.

### Phase 6 — Final polish

- [x] README with: description, stack, how to run locally, Scalar screenshots (placeholder in `README.md` + `JobTracker.http`)
- [x] Review status codes across all endpoints (added `ProducesResponseType` attributes, verified 201/200/204/400/404/409)
- [x] Review naming (en-US consistency in code, as that's the industry standard)
- [ ] Populate the database with my actual current job applications (skipped per request — manual via Scalar/`JobTracker.http`)

## 3. MVP Definition of Done

The project is ready for a portfolio when:

1. The full flow in `MVP.md` §2 runs without error via Scalar.
2. The 5 tests from Phase 5 pass.
3. There's a README clear enough for someone to run the project from scratch.
4. I can explain, without notes, the full path of a `PATCH /applications/{id}/status` — from the HTTP request to the response.
5. The code is on GitHub with incremental commit history (not a single giant commit) — that's also a signal to anyone evaluating it.

## 4. After the MVP (not now)

Valid ideas for a future iteration, **only after** the job situation is stable:

- Simple authentication (to eventually deploy and use from multiple devices)
- Lightweight React/TS frontend reusing what I already know
- Dashboard export to CSV/PDF
- Automatic reminders (background service) for applications with no response after N days

Recording these here instead of implementing them now is deliberate — the goal for October is being employable, not having a "complete" project.
