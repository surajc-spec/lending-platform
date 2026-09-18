using FluentAssertions;
using LendingPlatform.Domain.Enums;
using LendingPlatform.Domain.Services;

namespace LendingPlatform.Tests.Domain;

public class LoanDecisionEngineTests
{
    private readonly LoanDecisionEngine _engine = new();

    // -------------------------------------------------------
    // Loan amount boundaries
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldDecline_WhenLoanAmountIsBelowMinimum()
    {
        var result = _engine.Evaluate(
            loanAmount: 99_999m,
            assetValue: 200_000m,
            creditScore: 999);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    [Fact]
    public void Evaluate_ShouldNotDeclineDueToMinimum_WhenLoanAmountIsExactly100000()
    {
        var result = _engine.Evaluate(
            loanAmount: 100_000m,
            assetValue: 200_000m,
            creditScore: 750);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLoanAmountIsAboveMaximum()
    {
        var result = _engine.Evaluate(
            loanAmount: 1_500_001m,
            assetValue: 2_000_000m,
            creditScore: 999);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    [Fact]
    public void Evaluate_ShouldEvaluateNormally_WhenLoanAmountIsExactly1500000()
    {
        var result = _engine.Evaluate(
            loanAmount: 1_500_000m,
            assetValue: 2_500_000m,
            creditScore: 950);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    // -------------------------------------------------------
    // Loans below £1,000,000 - LTV below 60%
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldSucceed_WhenLoanBelow1Million_LtvBelow60_CreditScore750()
    {
        var result = _engine.Evaluate(
            loanAmount: 599_999m,
            assetValue: 1_000_000m,
            creditScore: 750);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLoanBelow1Million_LtvBelow60_CreditScore749()
    {
        var result = _engine.Evaluate(
            loanAmount: 599_999m,
            assetValue: 1_000_000m,
            creditScore: 749);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    // -------------------------------------------------------
    // Exactly 60% LTV
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldRequire800Credit_WhenLtvIsExactly60Percent()
    {
        var result = _engine.Evaluate(
            loanAmount: 600_000m,
            assetValue: 1_000_000m,
            creditScore: 800);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLtvIsExactly60PercentAndCreditScoreIs799()
    {
        var result = _engine.Evaluate(
            loanAmount: 600_000m,
            assetValue: 1_000_000m,
            creditScore: 799);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    // -------------------------------------------------------
    // Loans below £1,000,000 - LTV below 80%
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldSucceed_WhenLtvIsBelow80PercentAndCreditScoreIs800()
    {
        var result = _engine.Evaluate(
            loanAmount: 799_999m,
            assetValue: 1_000_000m,
            creditScore: 800);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLtvIsBelow80PercentAndCreditScoreIs799()
    {
        var result = _engine.Evaluate(
            loanAmount: 799_999m,
            assetValue: 1_000_000m,
            creditScore: 799);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    // -------------------------------------------------------
    // Exactly 80% LTV
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldRequire900Credit_WhenLtvIsExactly80Percent()
    {
        var result = _engine.Evaluate(
            loanAmount: 800_000m,
            assetValue: 1_000_000m,
            creditScore: 900);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLtvIsExactly80PercentAndCreditScoreIs899()
    {
        var result = _engine.Evaluate(
            loanAmount: 800_000m,
            assetValue: 1_000_000m,
            creditScore: 899);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    // -------------------------------------------------------
    // Loans below £1,000,000 - LTV below 90%
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldSucceed_WhenLtvIsBelow90PercentAndCreditScoreIs900()
    {
        var result = _engine.Evaluate(
            loanAmount: 899_999m,
            assetValue: 1_000_000m,
            creditScore: 900);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]

public void Evaluate_ShouldDecline_WhenLtvIsBelow90PercentAndCreditScoreIs899()
{
    var result = _engine.Evaluate(
        loanAmount: 899_999m,
        assetValue: 1_000_000m,
        creditScore: 899);

    result.Decision.Should().Be(LoanDecision.Declined);
    result.Reason.Should()
        .Be("For LTV below 90%, credit score must be at least 900.");
}

    // -------------------------------------------------------
    // Exactly 90% LTV
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldDecline_WhenLtvIsExactly90Percent()
    {
        var result = _engine.Evaluate(
            loanAmount: 900_000m,
            assetValue: 1_000_000m,
            creditScore: 999);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    // -------------------------------------------------------
    // Loans of £1,000,000 or more
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldSucceed_WhenLoanIsExactly1Million_LtvIs60Percent_CreditScoreIs950()
    {
        var result = _engine.Evaluate(
            loanAmount: 1_000_000m,
            assetValue: 1_666_666.6666666667m,
            creditScore: 950);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLoanIsExactly1Million_LtvIsAbove60Percent()
    {
        var result = _engine.Evaluate(
            loanAmount: 1_000_000m,
            assetValue: 1_600_000m,
            creditScore: 999);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    [Fact]
    public void Evaluate_ShouldDecline_WhenLoanIsExactly1Million_LtvIs60Percent_CreditScoreIs949()
    {
        var result = _engine.Evaluate(
            loanAmount: 1_000_000m,
            assetValue: 1_666_666.6666666667m,
            creditScore: 949);

        result.Decision.Should().Be(LoanDecision.Declined);
    }

    [Fact]
    public void Evaluate_ShouldSucceed_WhenLoanIsAbove1Million_AndCriteriaAreMet()
    {
        var result = _engine.Evaluate(
            loanAmount: 1_200_000m,
            assetValue: 2_000_000m,
            creditScore: 950);

        result.Decision.Should().Be(LoanDecision.Successful);
    }

    // -------------------------------------------------------
    // Asset value validation / divide-by-zero protection
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldThrow_WhenAssetValueIsZero()
    {
        var action = () => _engine.Evaluate(
            loanAmount: 500_000m,
            assetValue: 0m,
            creditScore: 800);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Asset value must be greater than zero*");
    }

    [Fact]
    public void Evaluate_ShouldThrow_WhenAssetValueIsNegative()
    {
        var action = () => _engine.Evaluate(
            loanAmount: 500_000m,
            assetValue: -1m,
            creditScore: 800);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Asset value must be greater than zero*");
    }

    // -------------------------------------------------------
    // LTV calculation
    // -------------------------------------------------------

    [Fact]
    public void Evaluate_ShouldCalculateLtvCorrectly()
    {
        var result = _engine.Evaluate(
            loanAmount: 500_000m,
            assetValue: 1_000_000m,
            creditScore: 800);

        result.Ltv.Should().Be(50m);
    }

    [Fact]
    public void Evaluate_ShouldNotRoundLtvBeforeDecision()
    {
        // Actual LTV = 59.999%
        // It is below 60%, so credit score 750 should succeed.

        var result = _engine.Evaluate(
            loanAmount: 599_990m,
            assetValue: 1_000_000m,
            creditScore: 750);

        result.Ltv.Should().Be(59.999m);
        result.Decision.Should().Be(LoanDecision.Successful);
    }
}