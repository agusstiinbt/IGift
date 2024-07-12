using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class Request : AuditableEntity<string>
    {
        public string IdUser { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Monto { get; set; }
        public required string Moneda { get; set; }
    }
}
