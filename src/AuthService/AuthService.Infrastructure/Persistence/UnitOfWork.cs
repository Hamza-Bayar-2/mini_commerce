using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _db.SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _db.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(ct);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(ct);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}