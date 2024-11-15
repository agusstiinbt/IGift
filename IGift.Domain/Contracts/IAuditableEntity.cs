﻿namespace IGift.Domain.Contracts
{
    public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
    {
    }

    public interface IAuditableEntity : IEntity
    {
        string CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        //string LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }
    }

    /// <summary>
    /// Quienes implementen este servicio no deberan poner implicitamente un Id
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
