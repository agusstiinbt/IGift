namespace IGift.Application.CQRS.Communication.Chat
{
    /// <summary>
    /// Clase para traer los chats con los que el usuario actual tiene guardados
    /// </summary>
    public class LoadChatUsers
    {
        /// <summary>
        /// Id del usuario en la sesion actual
        /// </summary>
        public string IdCurrentUser { get; set; } = string.Empty;
    }
}
