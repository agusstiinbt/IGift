using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Communication.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("LoadChatUsers")] // Ruta personalizada
        [AllowAnonymous]
        public async Task<ActionResult> LoadChatUsers(LoadChatUsers obj)
        {
            return Ok(await _chatService.LoadChatUsers(obj.IdCurrentUser));
        }

        [HttpPost("GetChatById")] // Ruta personalizada
        [AllowAnonymous]
        public async Task<ActionResult> GetChatById(GetChatById obj)
        {
            return Ok(await _chatService.GetChatHistoryByIdAsync(obj.UserId));
        }

        [HttpPost("SaveMessage")] // Ruta personalizada
        [AllowAnonymous]
        public async Task<ActionResult> SaveMessage(SaveChatMessage message)
        {
            return Ok(await _chatService.SaveMessage(message));
        }
    }
}
