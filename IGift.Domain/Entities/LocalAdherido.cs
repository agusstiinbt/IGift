using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class LocalAdherido : AuditableEntity<int>
    {
        public required string Nombre { get; set; }
        public bool Activo { get; set; }
        public string CreatedBy { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
