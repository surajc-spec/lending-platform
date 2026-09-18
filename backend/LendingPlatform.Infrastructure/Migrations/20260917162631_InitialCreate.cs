using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditScore = table.Column<short>(type: "smallint", nullable: false),
                    Ltv = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                    table.CheckConstraint("CK_LoanApplications_AssetValue_Positive", "[AssetValue] > 0");
                    table.CheckConstraint("CK_LoanApplications_CreditScore_Range", "[CreditScore] BETWEEN 1 AND 999");
                    table.CheckConstraint("CK_LoanApplications_Decision_Valid", "[Decision] IN ('Successful', 'Declined')");
                    table.CheckConstraint("CK_LoanApplications_LoanAmount_Positive", "[LoanAmount] > 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_CreatedAt",
                table: "LoanApplications",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanApplications");
        }
    }
}
