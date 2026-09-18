# AI Development Log

## 1. Purpose

This document outlines how artificial intelligence tools were utilized during the development of the Lending Platform technical assessment for Blackfinch.

In accordance with the assessment guidelines, AI was utilized as an assistive pair programmer / coding assistant. Final architectural decisions, requirements interpretation, technology choices, domain invariants, edge-case analysis, code review, manual verification, and final acceptance of code remained the candidate's responsibility. AI-generated suggestions were treated as preliminary drafts that were systematically reviewed, challenged, tested, and corrected prior to adoption.

---

## 2. How AI Was Used

AI assistance was selectively applied across the following development areas:

* **Requirements Clarification & Deconstruction**: Reviewing the assessment specification to isolate core domain inputs, outputs, LTV precision constraints, and underwriting rules.
* **Architecture & Project Structure Discussion**: Exploring project layer decomposition following Clean Architecture principles (`Domain`, `Application`, `Infrastructure`, `Api`, and `Tests`).
* **Coding Assistance**: Generating standard boilerplate implementations for DTOs, entity configurations, repository abstractions, and controller actions.
* **Domain Implementation Assistance**: Assisting with the initial structure of the domain decision engine (`LoanDecisionEngine`).
* **Test Case Suggestions**: Formulating exhaustive boundary test cases across credit scores, loan amounts, and LTV ratio thresholds.
* **Debugging & Problem Resolution**: Diagnosing test failures, syntax differences, and runtime configuration mismatches.
* **API Implementation Assistance**: Setting up FluentValidation validators and global exception handling middleware.
* **React Frontend Assistance**: Structuring modular UI components, React Router navigation, and REST API client services using Tailwind CSS.
* **Accessibility & UX Suggestions**: Adding keyboard focus states (`focus-visible`), ARIA attributes (`aria-invalid`, `aria-describedby`, `aria-busy`, `role="alert"`), and tabular number alignment.
* **Documentation Assistance**: Drafting technical explanations, API schemas, and production deployment considerations.

---

## 3. Key Prompt & Iteration Log

The table below summarizes the key interaction areas during development, documenting the prompt objective, AI assistance provided, and the candidate's critical review and actions taken:

| Area | Purpose of Prompt / Interaction | AI Assistance Provided | Candidate Review & Action Taken |
| :--- | :--- | :--- | :--- |
| **Requirements & Design** | Prompt objective: Analyze the Blackfinch technical brief and define the domain rules, boundaries, and architecture. | Proposed a 5-project Clean Architecture layout and summarized underwriting rules into a hierarchical decision flow. | Reviewed the boundaries. Established that unrounded decimal precision must be preserved for LTV evaluation, that declined applications must be persisted for metrics, and that invalid requests must be rejected without persistence. |
| **Domain Logic** | Prompt objective: Implement the `LoanDecisionEngine` to evaluate loan criteria and calculate LTV. | Drafted the decision engine logic and corresponding decline reasons. | **Critical Review**: Identified that the sub-90% LTV branch produced an inaccurate decline explanation. Corrected the text and strengthened the evaluation hierarchy. |
| **Database & Persistence** | Prompt objective: Configure EF Core entity mappings, SQL Server check constraints, and historical snapshot persistence. | Suggested entity mapping configurations for `LoanApplications` table including decimal precision and check constraints. | Verified that LTV snapshot precision (`decimal(9,4)`) and UTC timestamps (`SYSUTCDATETIME()`) were correctly mapped, and generated EF Core migrations. |
| **Application & Validation** | Prompt objective: Create DTOs and FluentValidation rules for incoming loan requests. | Generated request/response records and input validator rules. | Verified separation of concerns: ensured basic input format validation (e.g. positive amounts, credit score 1–999) remained distinct from domain business-rule decisions. |
| **REST API** | Prompt objective: Implement ASP.NET Core API controllers, exception middleware, and CORS policies. | Drafted `LoansController` with standard endpoints (`POST /api/loans`, `GET /api/loans`, `GET /api/loans/metrics`) and global error handling. | Reviewed error payload structures; verified that HTTP 400 validation responses return clear field errors and HTTP 500 hides technical internal stack traces. |
| **Automated Testing** | Prompt objective: Generate a comprehensive xUnit test suite covering business rules, boundaries, and integration flows. | Suggested boundary test cases for credit scores (749/750, 799/800, 899/900, 949/950), loan limits (£100k, £1m, £1.5m), and LTV limits. | Executed test suite (`dotnet test`), verified all 41 tests passed, and confirmed that tests accurately exercised the domain engine and API integration pipeline. |
| **Frontend Implementation** | Prompt objective: Implement a lightweight React application using Vite, Tailwind CSS, and React Router across 3 views. | Provided component scaffolding for `ApplyLoan`, `Applications`, and `Dashboard`. | Tested end-to-end integration against the running API; identified and corrected API property naming discrepancies on the dashboard. |
| **Accessibility & UX Polish** | Prompt objective: Review and enhance accessibility, focus management, and loading feedback without adding new features. | Suggested ARIA alert roles, focus-visible outlines, spinner states, and tabular number formatting. | Verified against accessibility guidelines: confirmed form inputs retain values after evaluation for user review, and verified keyboard tab navigation. |

---

## 4. Requirements and Architecture

AI assistance was used to explore layer boundaries and Clean Architecture separation. The candidate reviewed all proposals and made the final decisions regarding project structure and dependencies:

```text
backend/
├── LendingPlatform.Domain/          # Pure business rules, zero external dependencies
├── LendingPlatform.Application/     # DTOs, FluentValidation, use-case coordination, interfaces
├── LendingPlatform.Infrastructure/  # EF Core DbContext, SQL Server mapping, repository implementation
├── LendingPlatform.Api/             # Controllers, exception handling middleware, CORS, OpenAPI
└── LendingPlatform.Tests/           # xUnit test suite (domain, validation, API integration)

frontend/                            # React + Vite client (Apply Loan, Applications, Dashboard)
```

The candidate enforced that the `Domain` layer must remain strictly decoupled from ASP.NET Core, Entity Framework Core, SQL Server, and third-party UI libraries.

---

## 5. Domain Logic & Underwriting Rules

The core business logic resides in `LendingPlatform.Domain.Services.LoanDecisionEngine`. The engine applies the following evaluated rules:
1. **Loan Amount Limits**: Rejects loans $< \text{£100,000}$ or $> \text{£1,500,000}$.
2. **Loans of £1,000,000 or More**: Requires $\text{LTV} \le 60\%$ AND $\text{Credit Score} \ge 950$.
3. **Loans Below £1,000,000**:
   * $\text{LTV} < 60\% \implies \text{Credit Score} \ge 750$
   * $\text{LTV} < 80\% \implies \text{Credit Score} \ge 800$
   * $\text{LTV} < 90\% \implies \text{Credit Score} \ge 900$
   * $\text{LTV} \ge 90\% \implies \text{Declined}$

### Real Critical Correction: Sub-90% LTV Decline Reason
* **AI Suggestion**: In an early version of the decision engine, the branch evaluating sub-90% LTV with insufficient credit score generated a reason stating: *"Applications with an LTV of 90% or more are declined."*
* **Candidate Review**: The candidate questioned this explanation because an applicant with an LTV of 85% and a credit score of 850 was declined due to credit score, not because their LTV was 90% or higher.
* **Correction**: The candidate corrected the domain engine logic and message to:
  `"For LTV below 90%, credit score must be at least 900."`
* **Verification**: A dedicated unit test was added to verify this exact path, ensuring the engine returned the accurate explanation.

---

## 6. Testing Assistance & Verification

AI assistance was utilized to suggest boundary values for domain and integration test suites. The candidate executed, validated, and maintained the tests.

* **Total Backend Tests**: **41 passed, 0 failed, 0 skipped**.
* **Key Tested Boundaries**:
  * Credit score extremes: `1`, `999` (valid) vs `0`, `1000` (invalid, rejected at application boundary).
  * Credit score rule thresholds: `749` vs `750`, `799` vs `800`, `899` vs `900`, `949` vs `950`.
  * Loan amount limits: `£99,999` (declined), `£100,000` (eligible), `£999,999` (sub-million rules), `£1,000,000` (large loan rules), `£1,500,000` (upper limit), `£1,500,001` (declined).
  * LTV precision: Unrounded decimal precision (for example, an LTV value just below 60% such as 59.999% evaluated as < 60% without premature rounding).
  * Automatic decline at $\text{LTV} \ge 90\%$.
  * Persistence of declined applications to SQL Server.
  * Correctness of aggregated metric computations (sum of successful loans, mean LTV across all applications).

---

## 7. API / Backend Assistance

AI assistance supported the implementation of:
* DTO records (`CreateLoanApplicationRequest`, `LoanApplicationResponse`, `LoanMetricsResponse`).
* Repository abstractions (`ILoanApplicationRepository`) and EF Core implementation.
* `ExceptionHandlingMiddleware` for consistent error handling and hiding internal exception details.
* OpenAPI / Swagger configuration and CORS policy for local frontend access.

All generated code was audited against requirements to ensure no extraneous libraries or complexity (such as MediatR, CQRS, or JWT auth) were introduced.

---

## 8. Frontend Assistance

AI assistance was used to construct a clean, modular React frontend (Vite + Tailwind CSS):
* **Apply Loan**: Form with input validation, accessible alerts, loading state, and inline decision result card. Form values are intentionally kept visible post-evaluation for user inspection.
* **Applications**: Tabular view of historical applications fetched from `GET /api/loans` with decision badges, formatted GBP currency, and LTV percentages.
* **Dashboard**: Four aggregate metric cards matching the exact assessment output requirements.

---

## 9. Critical Review & Notable Corrections

A key evaluation criterion of this assessment is the ability to critically review and correct AI output. Below are real examples identified and resolved during development:

### Correction 1 — Inaccurate Decline Reason for Sub-90% LTV
* **Issue**: The generated engine initially used the $\ge 90\%$ decline text for applicants who had an LTV $< 90\%$ but failed the credit score requirement of 900.
* **Resolution**: Corrected the reason to explicitly state: `"For LTV below 90%, credit score must be at least 900."` Domain tests were updated and verified.

### Correction 2 — Dashboard Metric DTO Property Alignment
* **Issue**: During frontend integration, the dashboard component initially referenced property names that differed from the backend DTO (`successfulApplications`, `declinedApplications`, `totalLoanValueWritten`, `meanAverageLtv`), resulting in blank metric cards.
* **Resolution**: Identified through manual end-to-end testing against the live API. Corrected the component property mappings to accurately align with `LoanMetricsResponse`.

### Correction 3 — LTV Display vs. Business Rule Precision
* **Issue**: Initial suggestions recommended rounding LTV to 2 decimal places immediately upon calculation.
* **Resolution**: Rejected by the candidate. In lending systems, premature rounding alters boundary decisions (for example, an LTV value just below 60% could be displayed as 60.00% after rounding. Therefore, rounding must not occur before business-rule evaluation). The unrounded `decimal` is preserved for evaluation and persisted to SQL Server, with rounding applied solely for display.

---

## 10. Validation and Verification

AI suggestions were treated as preliminary drafts. Final validation was performed through deterministic tools and candidate inspection:
* **Backend Test Suite**: `dotnet test backend/LendingPlatform.slnx` $\implies$ **41/41 Passed**.
* **Frontend Code Quality**: `npm run lint` $\implies$ **0 errors, 0 warnings**.
* **Frontend Build**: `npm run build` $\implies$ **Production bundle generated successfully**.
* **Database Verification**: Inspected SQL Server tables via SSMS to verify snapshot LTV values, UTC dates, and check constraints.
* **End-to-End Verification**: Executed realistic loan applications through the browser UI and verified corresponding database updates and metric aggregations.

---

## 11. Summary of Candidate Critical Review

AI output was treated as a starting point rather than an authority. Generated code and suggestions were compared against the assessment requirements, reviewed for correctness, and verified through tests and manual execution. Where an inconsistency was found, the implementation was corrected before being accepted.
