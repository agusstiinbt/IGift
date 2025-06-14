namespace IGift.Application.Models.MongoDBModels.Chat
{
    public class ChatRoom
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? LastMessage { get; set; }
        public bool Seen { set; get; }
        public required string IdUser1 { get; set; }
        public required string IdUser2 { get; set; }
        public string? LastMessageFrom { get; set; }
    }
}
