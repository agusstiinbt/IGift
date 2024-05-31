using IGift.Domain.Entities;

namespace IGift.Application.Responses
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        //TODO [JsonIgnore]?
        public ICollection<GiftCard>? GiftCards { get; set; }
    }
}
