using LendingPlatform.Domain.Enums;

namespace LendingPlatform.Domain.Entities;

public class LoanApplication
{
    public int Id { get; private set; }

    public decimal LoanAmount { get; private set; }

    public decimal AssetValue { get; private set; }

    public int CreditScore { get; private set; }

    public decimal Ltv { get; private set; }

    public LoanDecision Decision { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    private LoanApplication()
    {
    }

    public LoanApplication(
        decimal loanAmount,
        decimal assetValue,
        int creditScore,
        decimal ltv,
        LoanDecision decision,
        string reason)
    {
        LoanAmount = loanAmount;
        AssetValue = assetValue;
        CreditScore = creditScore;
        Ltv = ltv;
        Decision = decision;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }
}