namespace IGift.Domain.Contracts
{
    /// <summary>
    /// Interfaz para entidades que pueden modificarse.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
    {
    }

    /// <summary>
    /// Interfaz para entidades que pueden modificarse.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IAuditableEntity : IEntity
    {
        string CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        string LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }
    }

    /// <summary>
    /// Interfaz para entidades que pueden modificarse.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
