using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class LocalAdherido : AuditableEntity<int>
    {
        public required string Nombre { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string ImageDataURL { get; set; } = string.Empty;
    }
}
