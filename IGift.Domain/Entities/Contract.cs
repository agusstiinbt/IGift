using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class Contract : AuditableEntity<int>
    {
        public required string IdUser1 { get; set; }
        public required string IdUser2 { get; set; }
    }
}
