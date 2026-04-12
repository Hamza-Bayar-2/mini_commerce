using FluentValidation;
using ProductService.Domain.Enums;

namespace ProductService.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();
        
        RuleFor(x => x.Name)
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(1200);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Stock.HasValue);

        RuleFor(x => x.StatusId)
            .Must(id => Enum.IsDefined(typeof(ProductStatuses), id!.Value))
            .WithMessage("Geçersiz ürün durumu.")
            .When(x => x.StatusId.HasValue);

        RuleFor(x => x)
            .Must(x => x.Stock != 0 || x.StatusId == (short)ProductStatuses.OUT_OF_STOCK)
            .WithMessage($"Stok 0 olduğunda durum '{ProductStatuses.OUT_OF_STOCK}' olmalıdır.")
            .When(x => x.Stock.HasValue && x.StatusId.HasValue);
    }
}