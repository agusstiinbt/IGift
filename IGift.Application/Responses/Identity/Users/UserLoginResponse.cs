using IGift.Domain.Entities;

namespace IGift.Application.Responses.Identity.Users
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string IdUser { get; set; }
        // [JsonIgnore] Acordarse la respuesta de Mauro sobre referencia circular.
        public ICollection<GiftCard>? GiftCards { get; set; }//TODO implementar en el TokenService el método de LogIn
    }
}
