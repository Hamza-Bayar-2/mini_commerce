using FluentValidation;

namespace ProductService.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(1200);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StatusId)
            .NotEmpty()
            .InclusiveBetween((short)1, (short)5).WithMessage("Invalid Status ID.")
            .Must((command, statusId) => command.Stock != 0 || statusId == 5)
            .WithMessage("If stock is 0, Status must be 'OUT_OF_STOCK' (5).");
    }
}