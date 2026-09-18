using LendingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LendingPlatform.Infrastructure.Persistence;

public class LendingDbContext : DbContext
{
    public LendingDbContext(DbContextOptions<LendingDbContext> options)
        : base(options)
    {
    }

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LendingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}