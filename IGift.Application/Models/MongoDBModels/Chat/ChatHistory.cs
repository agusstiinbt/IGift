using IGift.Application.Interfaces.Chat;

namespace IGift.Application.Models.MongoDBModels.Chat
{
    public class ChatHistory<TUser> : IChatHistory<TUser> where TUser : IChatUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string FromUserId { get; set; }
        public required string ToUserId { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public required bool Send { get; set; }
        public required bool Received { get; set; }
        public required bool Seen { get; set; }
        public TUser FromUser { get; set; }
        public TUser ToUser { get; set; }
    }
}
