namespace IGift.Application.Models.MongoDBModels.Chat
{
    /// <summary>
    /// Esta clase se usa para mostrar en el costado del chat room los chats que tenemos con otros usuarios
    /// </summary>
    public class ChatUserResponse
    {
        public string? LastMessage { get; set; }
        public bool Seen { get; set; }
        public bool IsLastMessageFromMe { get; set; }
        public byte[]? Data { get; set; }
        public string? UserName { get; set; }
        /// <summary>
        /// Este es el Id del otro usuario
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
