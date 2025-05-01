namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Esta clase va a ser responsable de mostrar los mensajes en el chat
    /// </summary>
    public class ChatHistoryResponse
    {
        //public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? NombreYApellido { get; set; } = string.Empty;
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Seen { get; set; }
        public bool Send { get; set; }
        public bool Received { get; set; }
        public DateTime Date { get; set; }
    }
}
