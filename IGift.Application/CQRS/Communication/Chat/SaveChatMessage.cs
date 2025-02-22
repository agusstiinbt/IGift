namespace IGift.Application.CQRS.Communication.Chat
{
    public class SaveChatMessage
    {
        public required string FromUserId { get; set; }
        public required string ToUserId { get; set; }
        /// <summary>
        /// No puede estar vacio o null
        /// </summary>
        public required string Message { get; set; }
    }
}
