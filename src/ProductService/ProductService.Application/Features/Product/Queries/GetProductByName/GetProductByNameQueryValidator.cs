using FluentValidation;

namespace ProductService.Application.Features.Product.Queries.GetProductByName;

public class GetProductByNameQueryValidator : AbstractValidator<GetProductByNameQuery>
{
    public GetProductByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2);
    }
}
