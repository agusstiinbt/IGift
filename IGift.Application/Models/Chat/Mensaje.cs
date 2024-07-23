namespace IGift.Application.Models.Chat
{
    public class Mensaje
    {
        public DateTime Date { get; set; }
        public string TextoMensaje { get; set; }
        public bool Enviado { get; set; }
        public bool Recibido { get; set; }
        public bool Visto { get; set; }
    }
}
