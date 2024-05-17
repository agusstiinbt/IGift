using IGift.Domain.Entities;

namespace IGift.Application.Responses
{
    public class ApplicationUserResponse
    {
        public ICollection<GiftCard>? GiftCards { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string ProfilePictureDataUrl { get; set; }
    }
}
