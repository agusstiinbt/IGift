using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Communication.Chat
{
    public interface IChatService
    {
        Task<IResult> SaveMessage(ChatHistory<IChatUser> message);

        /// <summary>
        /// Este metodo debe ser usado luego de haber traido todos los chats con otros usuarios. Cuando se haga click en un usuario se debe usar este metodo y pasarle el chatId correspondiente para traer la comunicacion entre dos users
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string chatId);

        /// <summary>
        /// Este metodo carga los bubbles del costado del chatroom. Solamente trae el ultimo mensaje para ser visto desde fuera.
        /// </summary>
        /// <returns></returns>
        Task<IResult<IEnumerable<ChatUsers>>> LoadChatUsers(string CurrentUserId);
    }
}
