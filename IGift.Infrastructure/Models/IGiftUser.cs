using IGift.Domain.Contracts;
using IGift.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace IGift.Infrastructure.Models
{
    public class IGiftUser : IdentityUser<string>, IAuditableEntity<string>
    {
        public ICollection<GiftCard> GiftCards { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string ProfilePictureDataUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [JsonIgnore]
        public ICollection<IGiftRole> Roles { get; set; }

        [JsonIgnore]
        public ICollection<IGiftUserRole> UsuariosRoles { get; set; }
        //TODO implementar para el chat
    }
}
 