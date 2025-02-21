using IGift.Application.Interfaces.Chat;

namespace IGift.Application.Models.Chat
{
    public class ChatHistory<TUser> : IChatHistory<TUser> where TUser : IChatUser
    {
        public long Id { get; set; }
        public required string FromUserId { get; set; }
        public required string ToUserId { get; set; }
        public required string Message { get; set; }
        public required bool Seen { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual required TUser FromUser { get; set; }
        public virtual required TUser ToUser { get; set; }
    }
}
