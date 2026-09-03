# Job Tracker — MVP

> A lightweight job application tracking API built with ASP.NET Core and PostgreSQL.

## 1. Goal

Build a small but realistic Web API that:

1. Solves a real, immediate problem: centralizing control of my job applications (currently scattered across LinkedIn, Indeed, ITJobs, emails, and loose notes).
2. Demonstrates, in a way I can defend in an interview, competence in: ASP.NET Core Web API, Dependency Injection, layered architecture, Entity Framework Core, PostgreSQL, DTOs, validation, error handling, HTTP semantics, and unit testing.

**Not a goal:** building a full recruitment system, a polished product, or anything requiring a frontend, authentication, or external integrations.

## 2. Definition of success

The MVP is done when, through Swagger, I can run this end-to-end flow without errors:

```
Create company → Create application → Save to PostgreSQL → Query
→ Filter → Change status → Update data → Delete → View dashboard
→ Correct HTTP errors at every step above
```

And I can explain, without hesitation, the full path of a request:

```
HTTP Request → Controller → DTO validation → Service → Business rules
→ EF Core → SQL → PostgreSQL → EF Core → DTO → HTTP Response
```

## 3. Domain model

```
Company
   │ 1
   │
   │ N
   ▼
JobApplication
```

A company has many applications. Nothing else in the MVP (no `User`, no `Note` as a separate entity, no `Location` as an entity).

### 3.1 Company

| Field     | Type     | Required | Rules                     |
| --------- | -------- | :------: | ------------------------- |
| Id        | int      |   auto   | —                         |
| Name      | string   |    ✅    | unique (case-insensitive) |
| Website   | string?  |    ❌    | valid URL, if provided    |
| CreatedAt | datetime |   auto   | —                         |
| UpdatedAt | datetime |   auto   | —                         |

### 3.2 JobApplication

| Field     | Type     | Required | Rules                                        |
| --------- | -------- | :------: | -------------------------------------------- |
| Id        | Guid     |   auto   | —                                            |
| CompanyId | int      |    ✅    | FK must exist                                |
| Position  | string   |    ✅    | —                                            |
| Status    | enum     |    ✅    | see 3.3                                      |
| Location  | string?  |    ❌    | free text (e.g. "Remote", "Hybrid - Lisbon") |
| SalaryMin | decimal? |    ❌    | ≥ 0                                          |
| SalaryMax | decimal? |    ❌    | ≥ 0 and ≥ SalaryMin                          |
| JobUrl    | string?  |    ❌    | valid URL, if provided                       |
| AppliedAt | date     |    ✅    | cannot be a future date                      |
| Notes     | string?  |    ❌    | free text                                    |
| CreatedAt | datetime |   auto   | —                                            |
| UpdatedAt | datetime |   auto   | —                                            |

### 3.3 ApplicationStatus (enum)

```
Interested → Applied → Screening → Interview → TechnicalTest → Offer
                  ↓          ↓           ↓              ↓
              Rejected   Rejected   Rejected      Rejected
                  ↓
              Withdrawn (from any active state)
```

Not a rigid state machine — just basic Service-level validation: don't allow a transition away from a terminal state (`Offer`, `Rejected`, `Withdrawn`) except explicit manual corrections via `PUT`.

## 4. API

Base path: `/api`

### 4.1 Companies

| Method | Route             | Description    | Success | Errors   |
| ------ | ----------------- | -------------- | ------- | -------- |
| POST   | `/companies`      | Create company | 201     | 400, 409 |
| GET    | `/companies`      | List companies | 200     | —        |
| GET    | `/companies/{id}` | Get company    | 200     | 404      |

### 4.2 Job Applications

| Method | Route                       | Description         | Success | Errors             |
| ------ | --------------------------- | ------------------- | ------- | ------------------ |
| POST   | `/applications`             | Create application  | 201     | 400, 404 (company) |
| GET    | `/applications`             | List (with filters) | 200     | —                  |
| GET    | `/applications/{id}`        | Get application     | 200     | 404                |
| PUT    | `/applications/{id}`        | Update data         | 200     | 400, 404           |
| PATCH  | `/applications/{id}/status` | Change status       | 200     | 400, 404           |
| DELETE | `/applications/{id}`        | Delete              | 204     | 404                |

**Supported filters on `GET /applications`:**

```
?status=Interview
?companyId=1
?from=2026-08-01&to=2026-08-31
```

Combinable (AND).

### 4.3 Dashboard

```
GET /api/dashboard
```

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

This endpoint is what justifies talking about "real queries and aggregations" in an interview — it's not just CRUD.

## 5. Validation rules (summary)

```
Company.Name              required, unique
JobApplication.CompanyId  required, must exist
JobApplication.Position   required
JobApplication.SalaryMax  >= SalaryMin (when both exist)
JobApplication.AppliedAt  cannot be in the future
URLs                      valid format when provided
```

## 6. Error contract (standard)

```json
{
  "status": 404,
  "message": "Job application not found."
}
```

Generated centrally by middleware — never scattered `try/catch` across controllers.

## 7. Out of scope (explicit)

```
Authentication / Authorization / JWT / Users
Frontend (React, etc.)
Mobile app
Email notifications
External integrations (LinkedIn, Indeed)
AI job matching / CV parsing
Docker, Kubernetes, Azure
Clean Architecture, CQRS, MediatR, generic Repository
```

Any of these is a future extension — never a prerequisite for the MVP.

## 8. What the project demonstrates on a resume

```
Backend      C#, ASP.NET Core Web API, EF Core, DI, DTOs, Validation, Exception Handling
Database     PostgreSQL, relational modeling, FKs, queries, aggregations, migrations
Engineering  Layered architecture, business logic, HTTP semantics, testing, Swagger, Git
```
