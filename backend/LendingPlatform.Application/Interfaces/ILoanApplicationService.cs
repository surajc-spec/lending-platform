using LendingPlatform.Application.DTOs;

namespace LendingPlatform.Application.Interfaces;

public interface ILoanApplicationService
{
    Task<LoanApplicationResponse> CreateAsync(
        CreateLoanApplicationRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LoanApplicationResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<LoanMetricsResponse> GetMetricsAsync(
        CancellationToken cancellationToken = default);
}