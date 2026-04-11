using MediatR;

namespace AuthService.Application.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }