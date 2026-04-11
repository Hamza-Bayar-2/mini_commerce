using MediatR;

namespace ProductService.Application.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }