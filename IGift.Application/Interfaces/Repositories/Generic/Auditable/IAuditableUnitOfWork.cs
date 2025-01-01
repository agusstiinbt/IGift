using IGift.Domain.Contracts;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Repositories.Generic.Auditable
{
    public interface IAuditableUnitOfWork<TId> : IDisposable
    {
        IAuditableRepository<T, TId> Repository<T>() where T : AuditableEntity<TId>;

        Task<IResult> Commit(string mensajeExito, CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}
