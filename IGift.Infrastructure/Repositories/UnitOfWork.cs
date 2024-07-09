using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace IGift.Infrastructure.Repositories
{
    public class UnitOfWork<TId> : IUnitOfWork<TId>
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;
        private bool disposed;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> Commit(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            throw new NotImplementedException();
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

        public IRepository<T, TId> Repository<T>() where T : AuditableEntity<TId>
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<,>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T, TId>)_repositories[type];
        }

        public Task Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
