using FluentValidation;
using LendingPlatform.Application.DTOs;

namespace LendingPlatform.Application.Validators;

public class CreateLoanApplicationValidator
    : AbstractValidator<CreateLoanApplicationRequest>
{
    public CreateLoanApplicationValidator()
    {
        RuleFor(x => x.LoanAmount)
            .GreaterThan(0)
            .WithMessage("Loan amount must be greater than zero.");

        RuleFor(x => x.AssetValue)
            .GreaterThan(0)
            .WithMessage("Asset value must be greater than zero.");

        RuleFor(x => x.CreditScore)
            .InclusiveBetween(1, 999)
            .WithMessage("Credit score must be between 1 and 999.");
    }
}