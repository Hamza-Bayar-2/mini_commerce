using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Features.Product.Commands.CreateProduct;
using ProductService.Application.Features.Product.Commands.UpdateProduct;
using ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;
using ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;
using ProductService.Application.Features.Product.Queries.GetAllProducts;
using ProductService.Application.Features.Product.Queries.GetProductById;
using ProductService.Application.Features.Product.Queries.GetProductByName;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create-product")]
    [Authorize(Policy = "SellerOrAdmin")]
    public async Task<IActionResult> CreateProductAsync(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPut("update-product")]
    [Authorize(Policy = "SellerOrAdmin")]
    public async Task<IActionResult> UpdateProductAsync(UpdateProductCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpDelete("soft-delete/{id}")]
    [Authorize(Policy = "SellerOrAdmin")]
    public async Task<IActionResult> SoftDeleteProductAsync(Guid id)
    {
        var result = await _mediator.Send(new SoftDeleteProductCommand(id));
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpDelete("hard-delete/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> HardDeleteProductAsync(Guid id)
    {
        var result = await _mediator.Send(new HardDeleteProductCommand(id));
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductByIdAsync(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        if (!result.IsSuccess)
            return NotFound(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("name/{name}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductByNameAsync(string name)
    {
        var result = await _mediator.Send(new GetProductByNameQuery(name));
        if (!result.IsSuccess)
            return NotFound(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProductsAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(), ct);
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.ErrorMessage });

        return Ok(result.Data);
    }
}