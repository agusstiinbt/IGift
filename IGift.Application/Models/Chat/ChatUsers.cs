namespace IGift.Application.Models.Chat
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
        public string ToUserId { get; set; } = string.Empty;
        public string FromUserId { get; set; } = string.Empty;
    }
}
