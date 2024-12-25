using IGift.Domain.Contracts;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Repositories
{
    public interface IUnitOfWork<TId> : IDisposable
    {
        IRepository<T, TId> Repository<T>() where T : AuditableEntity<TId>;

        Task<IResult> Commit(string mensajeExito, CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }

    public interface IUnitOfWork2<TId> : IDisposable
    {
        IRepository<T, TId> Repository<T>() where T : Entity<TId>;

        Task<IResult> Commit(string mensajeExito, CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}
