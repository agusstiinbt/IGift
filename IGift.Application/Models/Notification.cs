using IGift.Application.Enums;
using IGift.Domain.Contracts;

namespace IGift.Application.Models
{
    //TODO notification no debería de tener esta implementacion de Audit...
    public class Notification : AuditableEntity<string>
    {
        public string IdUser { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public TypeNotification Type { get; set; }
    }
}
