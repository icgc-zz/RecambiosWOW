namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Unit of Work pattern interface for handling transactions
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

