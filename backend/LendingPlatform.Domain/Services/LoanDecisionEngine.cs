using LendingPlatform.Domain.Enums;

namespace LendingPlatform.Domain.Services;

public class LoanDecisionEngine
{
    public LoanDecisionResult Evaluate(
        decimal loanAmount,
        decimal assetValue,
        int creditScore)
    {
        if (assetValue <= 0)
        {
            throw new ArgumentException(
                "Asset value must be greater than zero.",
                nameof(assetValue));
        }

        var ltv = (loanAmount / assetValue) * 100m;

        // Rule 1: Loan amount must be between £100,000 and £1,500,000.
        if (loanAmount < 100_000m)
        {
            return new LoanDecisionResult(
                LoanDecision.Declined,
                "Loan amount must be at least £100,000.",
                ltv);
        }

        if (loanAmount > 1_500_000m)
        {
            return new LoanDecisionResult(
                LoanDecision.Declined,
                "Loan amount must not exceed £1,500,000.",
                ltv);
        }

        // Rule 2: Loans of £1,000,000 or more.
        if (loanAmount >= 1_000_000m)
        {
            if (ltv <= 60m && creditScore >= 950)
            {
                return new LoanDecisionResult(
                    LoanDecision.Successful,
                    "Meets the lending criteria for loans of £1,000,000 or more.",
                    ltv);
            }

            return new LoanDecisionResult(
                LoanDecision.Declined,
                "For loans of £1,000,000 or more, LTV must be at most 60% and credit score must be at least 950.",
                ltv);
        }

        // Rule 3: Loans below £1,000,000.
        if (ltv < 60m)
        {
            return creditScore >= 750
                ? new LoanDecisionResult(
                    LoanDecision.Successful,
                    "Meets the lending criteria for LTV below 60%.",
                    ltv)
                : new LoanDecisionResult(
                    LoanDecision.Declined,
                    "For LTV below 60%, credit score must be at least 750.",
                    ltv);
        }

        if (ltv < 80m)
        {
            return creditScore >= 800
                ? new LoanDecisionResult(
                    LoanDecision.Successful,
                    "Meets the lending criteria for LTV below 80%.",
                    ltv)
                : new LoanDecisionResult(
                    LoanDecision.Declined,
                    "For LTV below 80%, credit score must be at least 800.",
                    ltv);
        }

      if (ltv < 90m)
{
    return creditScore >= 900
        ? new LoanDecisionResult(
            LoanDecision.Successful,
            "Meets the lending criteria for LTV below 90%.",
            ltv)
        : new LoanDecisionResult(
            LoanDecision.Declined,
            "For LTV below 90%, credit score must be at least 900.",
            ltv);
}

        return new LoanDecisionResult(
            LoanDecision.Declined,
            "Applications with an LTV of 90% or more are declined.",
            ltv);
    }
}

public record LoanDecisionResult(
    LoanDecision Decision,
    string Reason,
    decimal Ltv);