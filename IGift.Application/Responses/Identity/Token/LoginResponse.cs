using IGift.Domain.Entities;

namespace IGift.Application.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        //TODO [JsonIgnore]? Acordarse de implementar esto también?
        public ICollection<GiftCard>? GiftCards { get; set; }//TODO implementar en el TokenService el método de LogIn
    }
}
