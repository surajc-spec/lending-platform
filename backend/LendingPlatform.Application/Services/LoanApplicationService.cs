using FluentValidation;
using LendingPlatform.Application.DTOs;
using LendingPlatform.Application.Interfaces;
using LendingPlatform.Domain.Entities;
using LendingPlatform.Domain.Services;

namespace LendingPlatform.Application.Services;

public class LoanApplicationService : ILoanApplicationService
{
    private readonly ILoanApplicationRepository _repository;
    private readonly LoanDecisionEngine _decisionEngine;
    private readonly IValidator<CreateLoanApplicationRequest> _validator;

    public LoanApplicationService(
        ILoanApplicationRepository repository,
        LoanDecisionEngine decisionEngine,
        IValidator<CreateLoanApplicationRequest> validator)
    {
        _repository = repository;
        _decisionEngine = decisionEngine;
        _validator = validator;
    }

    public async Task<LoanApplicationResponse> CreateAsync(
        CreateLoanApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var decision = _decisionEngine.Evaluate(
            request.LoanAmount,
            request.AssetValue,
            request.CreditScore);

        var application = new LoanApplication(
            request.LoanAmount,
            request.AssetValue,
            request.CreditScore,
            decision.Ltv,
            decision.Decision,
            decision.Reason);

        await _repository.AddAsync(
            application,
            cancellationToken);

        return MapToResponse(application);
    }

    public async Task<IReadOnlyList<LoanApplicationResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var applications = await _repository.GetAllAsync(
            cancellationToken);

        return applications
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<LoanMetricsResponse> GetMetricsAsync(
        CancellationToken cancellationToken = default)
    {
        var successfulCount = await _repository
            .GetSuccessfulCountAsync(cancellationToken);

        var declinedCount = await _repository
            .GetDeclinedCountAsync(cancellationToken);

        var totalLoanValue = await _repository
            .GetTotalSuccessfulLoanValueAsync(cancellationToken);

        var meanAverageLtv = await _repository
            .GetMeanAverageLtvAsync(cancellationToken);

        return new LoanMetricsResponse(
            successfulCount,
            declinedCount,
            totalLoanValue,
            meanAverageLtv);
    }

    private static LoanApplicationResponse MapToResponse(
        LoanApplication application)
    {
        return new LoanApplicationResponse(
            application.Id,
            application.LoanAmount,
            application.AssetValue,
            application.CreditScore,
            application.Ltv,
            application.Decision.ToString(),
            application.Reason,
            application.CreatedAt);
    }
}