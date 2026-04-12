using FluentValidation;
using ProductService.Domain.Enums;

namespace ProductService.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(1200);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StatusId)
            .NotEmpty()
            .Must(id => Enum.IsDefined(typeof(ProductStatuses), id))
            .WithMessage("Geçersiz ürün durumu.");

        RuleFor(x => x)
            .Must(x => x.Stock != 0 || x.StatusId == (short)ProductStatuses.OUT_OF_STOCK)
            .WithMessage($"Stok 0 olduğunda durum '{ProductStatuses.OUT_OF_STOCK}' olmalıdır.");
    }
}