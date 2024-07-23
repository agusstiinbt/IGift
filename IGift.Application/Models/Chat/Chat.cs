namespace IGift.Application.Models.Chat
{
    public class Chat
    {
        public ICollection<Mensaje> Mensajes { get; set; }
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; }=string.Empty;
        /// <summary>
        /// Foto de perfil del otro usuario
        /// </summary>
        public string Imageurl { get; set; } = string.Empty;//TODO dejar esto?
    }
}
