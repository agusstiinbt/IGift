using IGift.Application.Enums;
using IGift.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace IGift.Application.Models
{
    public class Notification : AuditableEntity<string>
    {
        public string Id { get; set; }
        public string IdUser { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public TypeNotification Type { get; set; }
    }
}
