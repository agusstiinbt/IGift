﻿using IGift.Domain.Contracts;

namespace IGift.Application.Interfaces.Repositories
{
    public interface IUnitOfWork<TId>:IDisposable
    {
        IRepository<T, TId> Repository<T>() where T : AuditableEntity<TId>;

        Task<int> Commit(CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}
