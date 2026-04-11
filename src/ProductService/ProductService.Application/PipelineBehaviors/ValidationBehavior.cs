using ProductService.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace ProductService.Application.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any())
        {
            return await next(ct);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            var firstError = failures.First();
            var errorMessage = $"{firstError.PropertyName}: {firstError.ErrorMessage}";

            // Invoke static Failure method on Result<T> via reflection
            var failureMethod = typeof(TResponse).GetMethod("Failure", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if (failureMethod != null)
            {
                var failureResult = failureMethod.Invoke(null, new object[] { errorMessage });
                return (TResponse)failureResult!;
            }

            throw new ValidationException(failures);
        }

        return await next(ct);
    }
}