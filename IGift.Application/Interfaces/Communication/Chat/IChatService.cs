using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Communication.Chat
{
    public interface IChatService
    {
        Task<IResult> SaveMessage(SaveChatMessage saveChatMessage);

        /// <summary>
        /// Este metodo debe ser usado luego de haber traido todos los chats con otros usuarios. Cuando se haga click en un usuario se debe usar este metodo y pasarle el chatId correspondiente para traer la comunicacion entre dos users
        /// </summary>
        /// <param name="ToUserId"></param>
        /// <returns></returns>
        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string ToUserId);

        /// <summary>
        /// Este metodo carga los bubbles(chats historicos) del costado del chatroom. Solamente trae el ultimo mensaje para ser visto desde fuera.
        /// </summary>
        /// <returns></returns>
        Task<IResult<IEnumerable<ChatUser>>> LoadChatUsers(string CurrentUserId);
    }
}
