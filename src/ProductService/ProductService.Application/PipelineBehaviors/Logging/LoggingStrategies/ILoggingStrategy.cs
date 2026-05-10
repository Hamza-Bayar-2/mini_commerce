namespace ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public interface ILoggingStrategy<TRequest, TResponse>
{
    bool CanHandle(TRequest request);
    Task PublishLogAsync(TRequest request, TResponse response, CancellationToken ct);
}
