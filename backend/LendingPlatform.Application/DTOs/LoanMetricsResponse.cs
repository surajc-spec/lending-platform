namespace LendingPlatform.Application.DTOs;

public record LoanMetricsResponse(
    int SuccessfulApplications,
    int DeclinedApplications,
    decimal TotalLoanValueWritten,
    decimal MeanAverageLtv);