using FluentAssertions;
using FluentValidation;
using LendingPlatform.Application.DTOs;
using LendingPlatform.Application.Interfaces;
using LendingPlatform.Application.Services;
using LendingPlatform.Application.Validators;
using LendingPlatform.Domain.Entities;
using LendingPlatform.Domain.Enums;
using LendingPlatform.Domain.Services;

namespace LendingPlatform.Tests.Application;

public class LoanApplicationServiceTests
{
    private readonly LoanDecisionEngine _decisionEngine = new();
    private readonly CreateLoanApplicationValidator _validator = new();

    private LoanApplicationService CreateService(
        ILoanApplicationRepository repository)
    {
        return new LoanApplicationService(
            repository,
            _decisionEngine,
            _validator);
    }

    // -------------------------------------------------------
    // CreateAsync
    // -------------------------------------------------------

    [Fact]
    public async Task CreateAsync_ShouldCreateSuccessfulApplication()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var result = await service.CreateAsync(request);

        result.Decision.Should().Be("Successful");
        result.Ltv.Should().Be(50m);
        result.LoanAmount.Should().Be(500_000m);
        result.AssetValue.Should().Be(1_000_000m);
        result.CreditScore.Should().Be(800);

        repository.AddedApplication.Should().NotBeNull();
        repository.AddedApplication!.Decision
            .Should().Be(LoanDecision.Successful);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDeclinedApplication()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 700);

        var result = await service.CreateAsync(request);

        result.Decision.Should().Be("Declined");
        result.Ltv.Should().Be(50m);
        result.Reason.Should().NotBeNullOrWhiteSpace();

        repository.AddedApplication.Should().NotBeNull();
        repository.AddedApplication!.Decision
            .Should().Be(LoanDecision.Declined);
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectInvalidLoanAmount()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: -1m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var action = () => service.CreateAsync(request);

        await action.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*Loan amount must be greater than zero*");

        repository.AddedApplication.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectInvalidAssetValue()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 0m,
            CreditScore: 800);

        var action = () => service.CreateAsync(request);

        await action.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*Asset value must be greater than zero*");

        repository.AddedApplication.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectInvalidCreditScore()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 1000);

        var action = () => service.CreateAsync(request);

        await action.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*Credit score must be between 1 and 999*");

        repository.AddedApplication.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldAcceptCreditScoreAtMinimumBoundary()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 1);

        var action = () => service.CreateAsync(request);

        await action.Should().NotThrowAsync();

        repository.AddedApplication.Should().NotBeNull();
        repository.AddedApplication!.CreditScore.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldAcceptCreditScoreAtMaximumBoundary()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 999);

        var action = () => service.CreateAsync(request);

        await action.Should().NotThrowAsync();

        repository.AddedApplication.Should().NotBeNull();
        repository.AddedApplication!.CreditScore.Should().Be(999);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistDeclinedApplications()
    {
        var repository = new FakeLoanApplicationRepository();

        var service = CreateService(repository);

        var request = new CreateLoanApplicationRequest(
            LoanAmount: 900_000m,
            AssetValue: 1_000_000m,
            CreditScore: 999);

        var result = await service.CreateAsync(request);

        result.Decision.Should().Be("Declined");
        repository.AddedApplication.Should().NotBeNull();
    }

    // -------------------------------------------------------
    // GetAllAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedApplications()
    {
        var applications = new List<LoanApplication>
        {
            new(
                500_000m,
                1_000_000m,
                800,
                50m,
                LoanDecision.Successful,
                "Successful application"),

            new(
                900_000m,
                1_000_000m,
                999,
                90m,
                LoanDecision.Declined,
                "Applications with an LTV of 90% or more are declined.")
        };

        var repository = new FakeLoanApplicationRepository
        {
            Applications = applications
        };

        var service = CreateService(repository);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].Decision.Should().Be("Successful");
        result[1].Decision.Should().Be("Declined");
        result[0].Ltv.Should().Be(50m);
        result[1].Ltv.Should().Be(90m);
    }

    // -------------------------------------------------------
    // GetMetricsAsync
    // -------------------------------------------------------

    [Fact]
    public async Task GetMetricsAsync_ShouldReturnRepositoryMetrics()
    {
        var repository = new FakeLoanApplicationRepository
        {
            SuccessfulCount = 5,
            DeclinedCount = 3,
            TotalSuccessfulLoanValue = 2_500_000m,
            MeanAverageLtv = 62.5m
        };

        var service = CreateService(repository);

        var result = await service.GetMetricsAsync();

        result.SuccessfulApplications.Should().Be(5);
        result.DeclinedApplications.Should().Be(3);
        result.TotalLoanValueWritten.Should().Be(2_500_000m);
        result.MeanAverageLtv.Should().Be(62.5m);
    }

    // -------------------------------------------------------
    // Fake repository
    // -------------------------------------------------------

    private sealed class FakeLoanApplicationRepository
        : ILoanApplicationRepository
    {
        public LoanApplication? AddedApplication { get; private set; }

        public IReadOnlyList<LoanApplication> Applications { get; set; }
            = [];

        public int SuccessfulCount { get; set; }

        public int DeclinedCount { get; set; }

        public decimal TotalSuccessfulLoanValue { get; set; }

        public decimal MeanAverageLtv { get; set; }

        public Task AddAsync(
            LoanApplication loanApplication,
            CancellationToken cancellationToken = default)
        {
            AddedApplication = loanApplication;

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<LoanApplication>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Applications);
        }

        public Task<int> GetSuccessfulCountAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SuccessfulCount);
        }

        public Task<int> GetDeclinedCountAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DeclinedCount);
        }

        public Task<decimal> GetTotalSuccessfulLoanValueAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(TotalSuccessfulLoanValue);
        }

        public Task<decimal> GetMeanAverageLtvAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MeanAverageLtv);
        }
    }
}