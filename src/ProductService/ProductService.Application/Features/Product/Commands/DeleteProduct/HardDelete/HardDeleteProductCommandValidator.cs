using FluentValidation;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;

public class HardDeleteProductCommandValidator : AbstractValidator<HardDeleteProductCommand>
{
    public HardDeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}