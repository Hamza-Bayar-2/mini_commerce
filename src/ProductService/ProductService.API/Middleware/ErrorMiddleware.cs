using MassTransit;
using Shared.Events.Errors;

namespace ProductService.API.Middleware;

public class ErrorMiddleware : IMiddleware
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ErrorMiddleware(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            try
            {
                await _publishEndpoint.Publish(new ErrorOccurredEvent(
                    ServiceName: "ProductService",
                    Message: ex.Message,
                    StackTrace: ex.StackTrace,
                    CreatedAt: DateTime.UtcNow,
                    RequestPath: context.Request.Path,
                    RequestMethod: context.Request.Method
                ));
            }
            catch
            {
                
            }

            throw;
        }
    }
}
