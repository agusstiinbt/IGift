using IGift.Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace IGift.Infrastructure.Models
{
    public class IGiftRole : IdentityRole, IAuditableEntity<string>
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<IGiftRoleClaim> RoleClaims { get; set; }

        [JsonIgnore]
        public ICollection<IGiftUserRole> UsuariosRoles { get; set; }

        //TODO implementar el RoleClaims de blazorHeroContext

        public IGiftRole() : base()
        {
            RoleClaims = new HashSet<IGiftRoleClaim>();
        }

        public IGiftRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<IGiftRoleClaim>();
            Description = roleDescription;
        }
    }
}
