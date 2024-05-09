namespace IGift.Application.Responses
{
    public class TokenResponse//TODO fijarse si usar esto o LoginResult( también registerREsult)
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
