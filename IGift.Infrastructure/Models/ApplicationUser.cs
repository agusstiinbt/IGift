using IGift.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<GiftCard> GiftCards{ get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string ProfilePictureDataUrl { get; set; }
    }
}
