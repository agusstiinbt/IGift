using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class GiftCard : AuditableEntity<Guid>
    {
        public Guid Id { get; set; }   
        public int IdUser { get; set; }
        public int Monto { get; set; }
        public required string Moneda { get; set; }
        public bool IsActive { get; set; }
        public virtual LocalAdherido Local { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
