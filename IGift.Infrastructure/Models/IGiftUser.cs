using IGift.Application.Interfaces.Chat;
using IGift.Application.Models;
using IGift.Domain.Contracts;
using IGift.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure.Models
{
    public class IGiftUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        //public ProfilePicture? ProfilePictureDataUrl { get; set; }
        public string? ProfilePictureDataUrl { get; set; }


        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }


        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Peticiones> Pedidos { get; set; }
        public virtual ICollection<Contract> Contratos { get; set; }
        public virtual ICollection<OperacionesIntercambio> OperacionesIntercambios { get; set; }

        //Podemos implementar uan collection virtual de Chats pero no hace falta
    }
}
