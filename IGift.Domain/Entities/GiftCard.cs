using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class GiftCard : AuditableEntity<int>
    {
        public int IdUser { get; set; }
        public int Monto { get; set; }
        public required string Moneda { get; set; }
        public bool IsActive { get; set; }
        public virtual LocalAdherido Local { get; set; }
    }
}
