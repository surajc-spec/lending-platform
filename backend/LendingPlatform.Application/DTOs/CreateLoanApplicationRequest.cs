namespace LendingPlatform.Application.DTOs;

public record CreateLoanApplicationRequest(
    decimal LoanAmount,
    decimal AssetValue,
    int CreditScore);