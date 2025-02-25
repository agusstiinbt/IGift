namespace IGift.Application.Models.Chat
{
    /// <summary>
    /// Esta clase se usa para mostrar en el costado del chat room los chats que tenemos con otros usuarios
    /// </summary>
    public class ChatUserResponse
    {
        public string? LastMessage { get; set; }
        public bool Seen { get; set; }
        public string? LastMessageFrom { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
