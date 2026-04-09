using MediatR;

namespace AuthService.Application.Interfaces;

public interface ICommand<TResponse> : IRequest<TResponse> { }