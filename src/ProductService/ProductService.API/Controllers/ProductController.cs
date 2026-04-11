using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Features.Product.Commands.CreateProduct;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create-product")]
    [Authorize(Policy = "SellerOrAdmin")]
    public async Task<IActionResult> LoginAsync(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }
}