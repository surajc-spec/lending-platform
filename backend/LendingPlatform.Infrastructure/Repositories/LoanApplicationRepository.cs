using LendingPlatform.Application.Interfaces;
using LendingPlatform.Domain.Entities;
using LendingPlatform.Domain.Enums;
using LendingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Infrastructure.Repositories;

public class LoanApplicationRepository : ILoanApplicationRepository
{
    private readonly LendingDbContext _dbContext;

    public LoanApplicationRepository(LendingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        LoanApplication loanApplication,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.LoanApplications.AddAsync(
            loanApplication,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LoanApplication>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LoanApplications
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetSuccessfulCountAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LoanApplications
            .CountAsync(
                x => x.Decision == LoanDecision.Successful,
                cancellationToken);
    }

    public async Task<int> GetDeclinedCountAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LoanApplications
            .CountAsync(
                x => x.Decision == LoanDecision.Declined,
                cancellationToken);
    }

    public async Task<decimal> GetTotalSuccessfulLoanValueAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LoanApplications
            .Where(x => x.Decision == LoanDecision.Successful)
            .SumAsync(
                x => (decimal?)x.LoanAmount,
                cancellationToken) ?? 0m;
    }

    public async Task<decimal> GetMeanAverageLtvAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LoanApplications
            .Select(x => (decimal?)x.Ltv)
            .AverageAsync(cancellationToken) ?? 0m;
    }
}