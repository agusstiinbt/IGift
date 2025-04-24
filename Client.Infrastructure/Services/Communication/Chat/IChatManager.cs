using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Communication.Chat
{
    public interface IChatManager
    {
        /// <summary>
        /// Nos trae el chat completo seleccionado desed el cliente y pone como visto el ultimo mensaje
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatById(SearchChatById obj);


        /// <summary>
        /// Carga la grilla de chats en el costado izquierdo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<IResult<IEnumerable<ChatUserResponse>>> LoadChatUsers(LoadChatUsers obj);

        /// <summary>
        /// Guarda un mensaje al chat entre usuarios
        /// </summary>
        /// <param name="saveChatMessage"></param>
        /// <returns></returns>
        Task<IResult> SaveMessageAsync(SaveChatMessage saveChatMessage);
    }
}
