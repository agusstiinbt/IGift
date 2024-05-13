using IGift.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure.Models
{
    public class ApplicationUserRole : IdentityRole, IAuditableEntity<string>
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<BlazorHeroRoleClaim> RoleClaims { get; set; }
        //TODO implementar el RoleClaims de blazorHeroContext

        public ApplicationUserRole():base() 
        {
            RoleClaims=new HashSet<BlazorHeroRoleClaim>();  
        }

        public ApplicationUserRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<BlazorHeroRoleClaim>();
            Description = roleDescription;
        }
    }



    public class BlazorHeroRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
    {
        public string Description { get; set; }
        public string Group { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ApplicationUserRole Role { get; set; }

        public BlazorHeroRoleClaim() : base()
        {
        }

        public BlazorHeroRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}
