using System.Collections;
using System.ComponentModel.DataAnnotations;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;
using IGift.Shared.Wrapper;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Repositories.Generic.Auditable
{
    public class AuditableUnitOfWork<TId> : IAuditableUnitOfWork<TId>
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;
        private bool disposed;
        private readonly IAppCache _cache;

        public AuditableUnitOfWork(ApplicationDbContext context)
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

        public IAuditableRepository<T, TId> Repository<T>() where T : AuditableEntity<TId>
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(AuditableRepository<,>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IAuditableRepository<T, TId>)_repositories[type];
        }

        public Task Rollback()
        {
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }
    }

    public class CuentaBancaria : IEntity
    {
        public int Id { get; set; }
        public string Titular { get; set; }
        public decimal Saldo { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
    public class AppDbContext : DbContext
    {
        public DbSet<CuentaBancaria> CuentasBancarias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CuentaBancaria>()
                .Property(c => c.RowVersion)
                .IsRowVersion();
        }
    }

    public class CuentaBancariaService
    {
        private readonly AppDbContext _context;

        public CuentaBancariaService(AppDbContext context)
        {
            _context = context;
        }

        //Opcion optimista
        public async Task<bool> RetirarAsync(int cuentaId, decimal monto)
        {

            var cuenta = await _context.CuentasBancarias
                       .FirstOrDefaultAsync(c => c.Id == cuentaId);
            for (int intento = 0; intento < 3; intento++) // reintento en caso de conflicto
            {
                try
                {
                    if (cuenta == null)
                        return false;

                    if (cuenta.Saldo < monto)
                        return false;

                    cuenta.Saldo -= monto;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Alguien más modificó la cuenta al mismo tiempo
                    // Recargamos y reintentamos
                    var entry = _context.Entry(cuenta); /*le pide a Entity Framework que le devuelva el "seguimiento"(tracking) de esa entidad(cuenta) que ya estaba cargada.*/
                    await entry.ReloadAsync(); /*le dice "traé de nuevo esta entidad desde la base de datos, sobrescribiendo la que tengo en memoria".*/
                }
            }

            return false; // No se pudo por conflictos repetidos
        }

        //Opcion  pesimista:
        //            ¿Qué hacen esos locks?
        //WITH(UPDLOCK): pide un lock de actualización, que evita que otros lo lean con intenciones de escribir.

        //WITH(ROWLOCK): limita el lock solo a la fila, no a toda la tabla o página.

        //🔁 Resultado: si otro proceso intenta acceder a esa misma fila con un UPDLOCK o para escribir, espera hasta que el primero termine(COMMIT/ ROLLBACK). No se basa en tiempo: se basa en el lock real en la base.

        public async Task<bool> RetirarAsyncPesimista(int cuentaId, decimal monto)
        {
            // Tiempo máximo de espera por lock: 5 segundos
            _context.Database.SetCommandTimeout(5);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Bloqueamos la fila para escritura (UPDLOCK) y solo esa fila (ROWLOCK)
                var cuenta = await _context
                    .CuentasBancarias
                    .FromSqlRaw(@"
                    SELECT * FROM Cuentas WITH (UPDLOCK, ROWLOCK)
                    WHERE Id = {0}", cuentaId)
                    .FirstOrDefaultAsync();

                if (cuenta == null)
                    return false;

                if (cuenta.Saldo < monto)
                    return false;

                cuenta.Saldo -= monto;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Podés loguear o detectar si fue por timeout
                await transaction.RollbackAsync();
                return false;
            }
        }
    }

}
