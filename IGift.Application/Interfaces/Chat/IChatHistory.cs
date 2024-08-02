namespace IGift.Application.Interfaces.Chat
{
    public interface IChatHistory<TUser> where TUser : IChatUser
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
