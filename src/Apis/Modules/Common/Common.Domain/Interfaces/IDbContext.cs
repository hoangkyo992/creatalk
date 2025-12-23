using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Domain.Interfaces;

public interface IDbContext
{
    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    DatabaseFacade Database { get; }

    DbContext DbContext { get; }

    IDbContextTransaction GetCurrentTransaction();

    bool HasActiveTransaction { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task CommitTransactionAsync(IDbContextTransaction transaction);

    void RollbackTransaction();
}