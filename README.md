# Lending Platform — Technical Assessment

A full-stack lending platform simulation built for the **Blackfinch Engineering Candidate Technical Assessment (September 2026)**.

The platform provides an API and a web frontend to evaluate loan applications against strict lending rules, persist application records, and present aggregate portfolio metrics.

---

## 1. Assessment Overview & Requirements

The assessment evaluates:
* Correctness of domain logic and lending rules
* Clean code, separation of concerns, and modularity
* Sound domain modeling and invariant protection
* Effective AI collaboration, critical review of AI outputs, and documentation
* Explicit reasoning, assumptions, and trade-offs

Per the assessment brief, the solution is a focused technical demonstration and is not intended to be a finished production service.

### Inputs
Each loan application collects three inputs:
1. **Loan Amount**: Requested amount in GBP (£).
2. **Asset Value**: Value of the asset securing the loan in GBP (£).
3. **Credit Score**: Applicant credit score (integer between 1 and 999 inclusive).

### Outputs
* **Loan Decision**: `Successful` or `Declined` with a descriptive reason.
* **Applicant Summary**: Total applicants grouped by success status (`successfulApplications`, `declinedApplications`).
* **Total Loans Written**: Total value of loans written to date in GBP (£).
* **Mean Average LTV**: Mean average Loan-to-Value percentage across all applications.

---

## 2. Business Rules & Logic

### Loan-to-Value (LTV) Formula
```text
LTV = Loan Amount / Asset Value × 100
```

* LTV is calculated using full `decimal` precision in C#.
* Business rules are evaluated against the unrounded LTV value.
* LTV is rounded only for display/presentation purposes (formatted to two decimal places).

### Underwriting Criteria

1. **General Amount Limits**:
   * Decline if $\text{Loan Amount} < \text{£100,000}$
   * Decline if $\text{Loan Amount} > \text{£1,500,000}$

2. **Loans of £1,000,000 or More**:
   * Must satisfy **both**:
     * $\text{LTV} \le 60\%$
     * $\text{Credit Score} \ge 950$
   * Otherwise, the application is declined.

3. **Loans Below £1,000,000**:
   * If $\text{LTV} < 60\%$: Credit score must be $\ge 750$.
   * If $\text{LTV} < 80\%$: Credit score must be $\ge 800$.
   * If $\text{LTV} < 90\%$: Credit score must be $\ge 900$.
   * If $\text{LTV} \ge 90\%$: Declined automatically.

---

## 3. Important Business Assumptions

1. **Total Value of Loans Written to Date**:
   * Interpreted as the sum of `LoanAmount` for **Successful** applications only.
   * *Rationale*: A declined application does not result in a loan being written or capital disbursed.

2. **Mean Average LTV Across All Applications**:
   * Calculated as the arithmetic mean of `Ltv` across **all** persisted applications, including both successful and declined applications.
   * *Rationale*: The assessment explicitly specifies "across all applications."

3. **Persistence of Declined Applications**:
   * Valid applications that fail lending rules are evaluated as `Declined` and **are persisted** to the database.
   * *Rationale*: Historical application audits, status grouping, and mean average LTV require preserving all submitted applications.

4. **Rejection of Invalid Requests**:
   * Malformed or out-of-range requests (e.g. loan amount $\le 0$, asset value $\le 0$, credit score $< 1$ or $> 999$) fail validation and return HTTP `400 Bad Request`.
   * Invalid requests are **not persisted**.

---

## 4. Technology Stack

### Backend
* **Language & Runtime**: C#, .NET 10 (ASP.NET Core Web API)
* **Architecture**: Clean Architecture / Layered Domain Model
* **Data Access**: Entity Framework Core, SQL Server Provider
* **Validation**: FluentValidation
* **Documentation**: ASP.NET Core OpenAPI (`Microsoft.AspNetCore.OpenApi`)
* **Testing**: xUnit, FluentAssertions, `Microsoft.AspNetCore.Mvc.Testing`

### Frontend
* **Framework & Tooling**: React (v19), Vite
* **Language**: JavaScript (ES modules)
* **Styling**: Tailwind CSS
* **Routing**: React Router (v7)

### Database
* **Database Engine**: Microsoft SQL Server (Local instance: `localhost`)

---

## 5. System Architecture

The solution follows a Clean Architecture separation to ensure business logic remains decoupled from frameworks and persistence:

```text
Thrive-Task/
├── backend/
│   ├── LendingPlatform.slnx
│   ├── LendingPlatform.Domain/          # Core entities, enums, LoanDecisionEngine, domain logic
│   ├── LendingPlatform.Application/     # DTOs, FluentValidation validators, service interfaces & use-case orchestration
│   ├── LendingPlatform.Infrastructure/  # EF Core DbContext, entity mapping, migrations, repositories
│   ├── LendingPlatform.Api/             # ASP.NET Core Web API, controllers, middleware, CORS, OpenAPI
│   └── LendingPlatform.Tests/           # xUnit tests covering domain, application, validation, and API integration
└── frontend/
    ├── index.html
    ├── package.json
    ├── vite.config.js
    └── src/
        ├── components/                  # LoanForm, DecisionCard, ApplicationTable, MetricCard
        ├── layouts/                     # AppLayout with navigation
        ├── pages/                       # ApplyLoan, Applications, Dashboard
        ├── services/                    # loanApi.js (Fetch client)
        └── ...
```

### Layer Dependency Direction
$$\text{Domain} \leftarrow \text{Application} \leftarrow \text{Infrastructure} \leftarrow \text{API}$$

* **Domain**: Contains zero dependencies on external frameworks or databases. Encapsulates `LoanApplication`, `LoanDecision`, and `LoanDecisionEngine`.
* **Application**: Orchestrates request processing, validates input boundaries using FluentValidation, defines repository contracts (`ILoanApplicationRepository`), and handles DTO mapping.
* **Infrastructure**: Implements `LendingDbContext` and persistence repositories using Entity Framework Core for SQL Server.
* **API**: Exposes HTTP endpoints, configures CORS, hosts global exception handling middleware, and maps API routes.
* **Frontend**: Three dedicated views communicating with the backend via REST endpoints.

---

## 6. Database Design & Persistence

* **Database Name**: `LendingPlatformDb` (Integration tests use `LendingPlatform_IntegrationTests`)
* **Primary Table**: `LoanApplications`

### Schema Details
| Column | Type | Nullable | Details / Constraints |
| :--- | :--- | :--- | :--- |
| `Id` | `INT` | No | Primary Key, Identity `(1,1)` |
| `LoanAmount` | `DECIMAL(18,2)` | No | Check constraint: `LoanAmount > 0` |
| `AssetValue` | `DECIMAL(18,2)` | No | Check constraint: `AssetValue > 0` |
| `CreditScore` | `SMALLINT` | No | Check constraint: `CreditScore BETWEEN 1 AND 999` |
| `Ltv` | `DECIMAL(9,4)` | No | Calculated application snapshot |
| `Decision` | `NVARCHAR(20)` | No | Check constraint: `Decision IN ('Successful', 'Declined')` |
| `Reason` | `NVARCHAR(250)` | No | Plain-text explanation of decision |
| `CreatedAt` | `DATETIME2(7)` | No | SQL UTC Default: `SYSUTCDATETIME()`, Index: `IX_LoanApplications_CreatedAt` |

* **Snapshot LTV**: Calculated LTV is persisted directly to preserve the historical evaluation snapshot.

---

## 7. REST API Endpoints

Base URL: `http://localhost:5107/api`

### 1. Submit Loan Application
* **Endpoint**: `POST /api/loans`
* **Description**: Validates input, evaluates underwriting rules, persists the application to SQL Server, and returns the evaluation outcome.
* **Request Body**:
```json
{
  "loanAmount": 500000.00,
  "assetValue": 1000000.00,
  "creditScore": 800
}
```
* **Successful Response (`201 Created`)**:
```json
{
  "id": 1,
  "loanAmount": 500000.00,
  "assetValue": 1000000.00,
  "creditScore": 800,
  "ltv": 50.0,
  "decision": "Successful",
  "reason": "Meets the lending criteria for LTV below 60%.",
  "createdAt": "2026-09-18T10:30:00Z"
}
```
* **Declined Response (`201 Created` - Persisted)**:
```json
{
  "id": 2,
  "loanAmount": 900000.00,
  "assetValue": 1000000.00,
  "creditScore": 999,
  "ltv": 90.0,
  "decision": "Declined",
  "reason": "Applications with an LTV of 90% or more are declined.",
  "createdAt": "2026-09-18T10:31:00Z"
}
```
* **Validation Error (`400 Bad Request` - Not Persisted)**:
```json
{
  "message": "One or more validation errors occurred.",
  "errors": {
    "CreditScore": [
      "Credit score must be between 1 and 999."
    ]
  }
}
```

### 2. Retrieve Historical Applications
* **Endpoint**: `GET /api/loans`
* **Description**: Returns all historical applications ordered from newest to oldest.
* **Response (`200 OK`)**: Array of application objects.

### 3. Retrieve Platform Metrics
* **Endpoint**: `GET /api/loans/metrics`
* **Description**: Computes aggregate metrics from stored loan applications.
* **Response (`200 OK`)**:
```json
{
  "successfulApplications": 5,
  "declinedApplications": 5,
  "totalLoanValueWritten": 2600000.00,
  "meanAverageLtv": 63.50
}
```

---

## 8. Web Frontend

The user interface consists of three views accessible from the top navigation bar:

1. **Apply Loan (`/`)**:
   * Interactive form to enter Loan Amount, Asset Value, and Credit Score.
   * Client-side validation with immediate feedback and accessible field alerts.
   * Real-time submission displaying the evaluated decision badge, formatted values, and underwriting reason.
   * Form inputs remain visible after evaluation for easy review against the outcome.
2. **Applications (`/applications`)**:
   * Tabular list of historical loan applications showing ID, Loan Amount, Asset Value, Credit Score, LTV, Decision badge, and submission date.
   * Handles empty, loading, and error states gracefully.
3. **Dashboard (`/dashboard`)**:
   * Summary view presenting the four required platform metrics:
     * Successful Applications count
     * Declined Applications count
     * Total Loans Written (£)
     * Mean Average LTV (%)

*Note: User authentication and role-based access control were intentionally excluded from the assessment scope.*

---

## 9. Local Setup & Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Node.js (v18+) & npm](https://nodejs.org/)
* [Microsoft SQL Server](https://www.microsoft.com/sql-server/) running locally (instance accessible via `localhost` or Windows Authentication)
* `dotnet-ef` CLI tool (optional, for running migrations manually: `dotnet tool install --global dotnet-ef`)

### 1. Database Setup
The connection string in `backend/LendingPlatform.Api/appsettings.json` is configured as:
```text
Server=localhost;Database=LendingPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;
```

To apply migrations and create the `LendingPlatformDb` database and schema:
```powershell
dotnet ef database update --project .\backend\LendingPlatform.Infrastructure\LendingPlatform.Infrastructure.csproj --startup-project .\backend\LendingPlatform.Api\LendingPlatform.Api.csproj
```

### 2. Running the Backend API
In a new terminal window:
```powershell
cd backend
dotnet run --project .\LendingPlatform.Api\LendingPlatform.Api.csproj
```
The API starts locally at:
* **API URL**: `http://localhost:5107`
* **OpenAPI endpoint**: `http://localhost:5107/openapi/v1.json`

### 3. Running the Frontend
In a second terminal window:
```powershell
cd frontend
npm install
npm run dev
```
The Vite development server will start (typically at `http://localhost:5173`). Open the browser link to interact with the platform.

---

## 10. Verification & Test Execution

### Backend Automated Tests
The backend test suite contains **41 automated tests** covering:
* Domain business rules, LTV calculations, and engine decisions
* Boundary conditions (e.g. £100k, £1m, £1.5m, 60%, 80%, 90% LTV, credit score boundaries)
* FluentValidation rules for malformed requests
* Application services and repository coordination
* End-to-end API integration tests (`WebApplicationFactory`)
* Database persistence and aggregated metric calculations

To run the backend test suite:
```powershell
dotnet test backend/LendingPlatform.slnx
```

**Verified Test Result**:
```text
Passed!  - Failed: 0, Passed: 41, Skipped: 0, Total: 41
```

### Frontend Verification
The frontend code adheres to standard ESLint rules and production build bundling:
```powershell
cd frontend
npm run lint
npm run build
```
Both commands pass without warnings or errors.

---

## 11. AI-Assisted Development

AI tools were used as coding assistants throughout the development process. They were used to support implementation, debugging, test-writing suggestions, code review, and documentation.

The architecture, technology choices, interpretation of the lending rules, engineering decisions, testing strategy, verification, and final acceptance of changes remained the candidate's responsibility.

AI-generated suggestions were reviewed and tested rather than being accepted blindly. Notable iterations and corrections are documented in `AI_LOG.md`.

---

## 12. Production Considerations

While this assessment demonstrates clean architecture, well-tested domain rules, and resilient persistence, several enhancements would be considered prior to deploying this system to a production environment:

1. **Authentication & Authorization**:
   * Implement secure identity provider integration (e.g. OAuth2 / OpenID Connect) to safeguard financial applications and restrict aggregate metric dashboards.
2. **Data Privacy & PII Handling**:
   * Secure storage and encryption-at-rest for applicant personal identifying information (PII) to comply with GDPR and financial data regulations.
3. **Transport Security**:
   * Enforce HTTPS redirection and strict HTTP Strict Transport Security (HSTS) headers.
4. **Secret Management**:
   * Extract database connection strings and environment keys into Azure Key Vault, AWS Secrets Manager, or environment variables instead of source-controlled configuration.
5. **Observability & Logging**:
   * Implement structured logging (e.g. Serilog), distributed tracing (OpenTelemetry), and health check endpoints (`/health`).
6. **Rate Limiting & Security Hardening**:
   * Implement rate-limiting middleware to protect the loan evaluation endpoint from denial-of-service or automated score enumeration attempts.
7. **Resilience & Caching**:
   * Introduce read caching for dashboard metrics where real-time aggregation across large historical tables may become resource-intensive.
