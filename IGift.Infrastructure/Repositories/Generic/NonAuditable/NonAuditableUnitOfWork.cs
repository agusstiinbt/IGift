using System.Collections;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;
using IGift.Shared.Wrapper;
using LazyCache;

namespace IGift.Infrastructure.Repositories.Generic.NonAuditable
{
    public class NonAuditableUnitOfWork<TId> : INonAuditableUnitOfWork<TId>
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;
        private bool disposed;
        private readonly IAppCache _cache;

        public NonAuditableUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Commit(string mensajeExito, CancellationToken cancellationToken)
        {
            int affectedRows = await _context.SaveChangesAsync(cancellationToken);

            if (affectedRows > 0)
            {
                return await Result.SuccessAsync(mensajeExito);
            }
            else
            {
                return await Result.FailAsync("No changes were made to the database.");
            }
        }

        public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            foreach (var cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
            return result;
        }

        //TODO averiguar quién está llamando al dispose y cómo lo hace
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _context.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public INonAuditableRepository<T, TId> Repository<T>() where T : Entity<TId>
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(NonAuditableRepository<,>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (INonAuditableRepository<T, TId>)_repositories[type];
        }

        public Task Rollback()
        {
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }
    }
}
