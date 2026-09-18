using LendingPlatform.Domain.Entities;

namespace LendingPlatform.Application.Interfaces;

public interface ILoanApplicationRepository
{
    Task AddAsync(
        LoanApplication loanApplication,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LoanApplication>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<int> GetSuccessfulCountAsync(
        CancellationToken cancellationToken = default);

    Task<int> GetDeclinedCountAsync(
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSuccessfulLoanValueAsync(
        CancellationToken cancellationToken = default);

    Task<decimal> GetMeanAverageLtvAsync(
        CancellationToken cancellationToken = default);
}