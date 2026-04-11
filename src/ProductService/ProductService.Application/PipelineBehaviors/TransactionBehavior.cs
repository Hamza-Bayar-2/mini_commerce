using ProductService.Application.Interfaces;
using MediatR;

namespace ProductService.Application.PipelineBehaviors;

public class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Skip for queries, only apply to commands
        if (request is not ICommand<TResponse>)
            return await next(ct);

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var response = await next(ct);

            // Check if response is a Result type and represents a failure
            if (response is IResult result && !result.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                return response;
            }

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            return response;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}