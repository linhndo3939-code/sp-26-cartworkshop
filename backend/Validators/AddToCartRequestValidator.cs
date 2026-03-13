using FluentValidation;
using backend.Dtos;

namespace backend.Validators;

public class AddToCartRequestValidator : AbstractValidator<AddToCartRequest>
{
    public AddToCartRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be a positive number.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 99)
            .WithMessage("You can only add between 1 and 99 items at a time.");
    }
}