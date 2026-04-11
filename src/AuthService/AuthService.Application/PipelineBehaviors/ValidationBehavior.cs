using AuthService.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace AuthService.Application.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            if (typeof(IResult).IsAssignableFrom(typeof(TResponse)))
            {
                var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));

                // Result<T>.Failure metodunu çağırıyoruz
                var failureMethod = typeof(TResponse).GetMethod("Failure");
                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, [errorMessage])!;
                }
            }

            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}