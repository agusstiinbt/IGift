using IGift.Domain.Entities;

namespace IGift.Application.Responses
{
    public class ApplicationUserResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<GiftCard>? GiftCards { get; set; }
    }
}
