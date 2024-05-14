using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure.Models
{
    public class IGiftUserRole : IdentityUserRole<string>
    {
        public IGiftUser Usuario { get; set; }

        public IGiftRole Roles { get; set; }
    }
}
