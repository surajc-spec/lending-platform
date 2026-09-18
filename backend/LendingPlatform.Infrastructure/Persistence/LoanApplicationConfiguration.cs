using LendingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LendingPlatform.Infrastructure.Persistence;

public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.ToTable("LoanApplications", table =>
        {
            table.HasCheckConstraint(
                "CK_LoanApplications_LoanAmount_Positive",
                "[LoanAmount] > 0");

            table.HasCheckConstraint(
                "CK_LoanApplications_AssetValue_Positive",
                "[AssetValue] > 0");

            table.HasCheckConstraint(
                "CK_LoanApplications_CreditScore_Range",
                "[CreditScore] BETWEEN 1 AND 999");

            table.HasCheckConstraint(
                "CK_LoanApplications_Decision_Valid",
                "[Decision] IN ('Successful', 'Declined')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LoanAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.AssetValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.CreditScore)
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(x => x.Ltv)
            .HasColumnType("decimal(9,4)")
            .IsRequired();

        builder.Property(x => x.Decision)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.HasIndex(x => x.CreatedAt);
    }
}