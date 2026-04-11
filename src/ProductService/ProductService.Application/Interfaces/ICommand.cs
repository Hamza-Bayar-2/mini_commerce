using MediatR;

namespace ProductService.Application.Interfaces;

public interface ICommand<TResponse> : IRequest<TResponse> { }