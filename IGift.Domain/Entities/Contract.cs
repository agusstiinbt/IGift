using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class Contract : AuditableEntity<int>
    {
        public int IdUser1 { get; set; }
        public int IdUser2 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
