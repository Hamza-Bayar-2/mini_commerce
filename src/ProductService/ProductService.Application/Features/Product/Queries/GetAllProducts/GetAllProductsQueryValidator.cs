using FluentValidation;

namespace ProductService.Application.Features.Product.Queries.GetAllProducts;

public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .NotNull()
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PageSize)
            .NotNull()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Search)
            .MaximumLength(100);
    }
}