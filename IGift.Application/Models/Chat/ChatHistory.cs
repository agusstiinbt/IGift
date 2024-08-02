using IGift.Application.Interfaces;
using IGift.Application.Interfaces.Chat;

namespace IGift.Application.Models.Chat
{
    public class ChatHistory<TUser> : IChatHistory<TUser> where TUser : IChatUser
    {
        public long Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
