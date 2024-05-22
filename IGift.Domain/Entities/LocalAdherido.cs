using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class LocalAdherido : IAuditableEntity<int>
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public bool Activo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
