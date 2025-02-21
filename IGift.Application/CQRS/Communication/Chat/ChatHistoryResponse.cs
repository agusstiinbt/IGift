namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Esta clase va a ser responsable de mostrar los mensajes en el chat
    /// </summary>
    public class ChatHistoryResponse
    {
        //public long Id { get; set; }
        public string FromUserId { get; set; } = string.Empty;
        //public string FromUserImageURL { get; set; } = string.Empty;
        public string FromUserFullName { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        //public string ToUserImageURL { get; set; } = string.Empty;
        public string ToUserFullName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
