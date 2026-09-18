namespace LendingPlatform.Application.DTOs;

public record LoanApplicationResponse(
    int Id,
    decimal LoanAmount,
    decimal AssetValue,
    int CreditScore,
    decimal Ltv,
    string Decision,
    string Reason,
    DateTime CreatedAt);