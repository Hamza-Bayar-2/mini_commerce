using FluentValidation;

namespace ProductService.Application.Features.Product.Queries.GetAllProducts;

public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        // No validation rules necessary for retrieving all products without pagination.
    }
}