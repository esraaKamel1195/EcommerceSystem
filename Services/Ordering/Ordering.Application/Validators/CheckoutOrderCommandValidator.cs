using FluentValidation;
using Ordering.Application.Commands;

namespace Ordering.Application.Validators
{
    public class CheckoutOrderCommandValidator: AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(o => o.UserName)
                .NotEmpty()
                .WithMessage("{UserName} is required.")
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{UserName} must not exceed 70 characters");

            RuleFor(o => o.TotalPrice)
                .NotEmpty()
                .WithMessage("{TotalPrice} is required.")
                .GreaterThan(0)
                .WithMessage("{TotalPrice} must be greater than zero.");

            RuleFor(o => o.Email)
                .NotEmpty()
                .WithMessage("{Email} is required.");

            RuleFor(o => o.FirstName)
                .NotEmpty()
                .WithMessage("{FirstName} is required.")
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{FirstName} must not exceed 70 characters");

            RuleFor(o => o.LastName)
                .NotEmpty()
                .NotNull()
                .WithMessage("{LastName} is required.");
        }
    }
}
