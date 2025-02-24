namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Clase usada para enviar un mensaje a un chat con un usuario paticular
    /// </summary>
    public class SaveChatMessage
    {
        /// <summary>
        /// El Id del usuario actual en la sesion del programa
        /// </summary>
        public required string FromUserId { get; set; }

        /// <summary>
        /// El Id del usuario al cual queremos enviar el mensaje
        /// </summary>
        public required string ToUserId { get; set; }

        /// <summary>
        /// No puede estar vacio o null
        /// </summary>
        public required string Message { get; set; }
    }
}
