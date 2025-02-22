namespace IGift.Application.Models.Chat
{
    /// <summary>
    /// Esta clase se usa para mostrar en el costado del chat room los chats que tenemos con otros usuarios
    /// </summary>
    public class ChatUser
    {
        /// <summary>
        /// El Id del chat correspondiente
        /// </summary>
        public string ChatId { get; set; }

        /// <summary>
        /// Este es el Id del usuario con el cual estamos chateando
        /// </summary>
        public string ToUserId { get; set; }
        public required string UserName { get; set; }
        public string? UserProfileImageUrl { get; set; }
        public required string LastMessage { get; set; }
        public bool Seen { get; set; }
    }
}
