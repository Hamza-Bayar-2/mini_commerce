using FluentValidation;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;

public class SoftDeleteProductCommandValidator : AbstractValidator<SoftDeleteProductCommand>
{
    public SoftDeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}