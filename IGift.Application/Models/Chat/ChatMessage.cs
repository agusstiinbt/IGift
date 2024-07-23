namespace IGift.Application.Models.Chat
{
    public class ChatMessage
    {
        public DateTime Date { get; set; }
        public string Mensaje { get; set; }
        public bool Enviado { get; set; }
        public bool Recibido { get; set; }
        public bool Visto { get; set; }
    }
}
