using ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;
using MediatR;
using ProductService.Application.Interfaces;

namespace ProductService.Application.PipelineBehaviors;

public class LoggingBehaviors<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<ILoggingStrategy<TRequest, TResponse>> _strategies;

    public LoggingBehaviors(IEnumerable<ILoggingStrategy<TRequest, TResponse>> strategies)
    {
        _strategies = strategies;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next(ct);

        // If the result is not successful, we don't log the event
        if (response is IResult { IsSuccess: false })
            return response;

        try
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(request));
            if (strategy != null)
            {
                await strategy.PublishLogAsync(request, response, ct);
            }
        }
        catch (Exception)
        {
            // errors are handled in errormidleware
        }

        return response;
    }
}
