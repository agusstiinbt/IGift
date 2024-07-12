using IGift.Application.Models;
using IGift.Domain.Contracts;
using IGift.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure.Models
{
    public class IGiftUser : IdentityUser<string?>, IAuditableEntity<string?>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }


        public string? RefreshToken { get; set; }
        public string? ProfilePictureDataUrl { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }


        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public bool IsActive { get; set; }

        public List<Notification> Notifications { get; set; } = new List<Notification>() { };
        public List<Request> Pedidos { get; set; } = new List<Request>() { };

        //TODO implementar para el chat
        public IGiftUser()
        {

        }
    }
}
