using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Communication.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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

        [HttpPost("LoadChatUsers")]
        public async Task<ActionResult> LoadChatUsers(LoadChatUsers obj)
        {
            try
            {
                var result = await _chatService.LoadChatUsers(obj.IdCurrentUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Excepción en LoadChatUsers para el usuario {UserId}", obj.IdCurrentUser);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("GetChatById")]
        public async Task<ActionResult> GetChatById(SearchChatById obj)
        {
            return Ok(await _chatService.GetChatHistoryByIdAsync(obj));
        }

        [HttpPost("SaveMessage")]
        public async Task<ActionResult> SaveMessage(SaveChatMessage message)
        {
            return Ok(await _chatService.SaveMessage(message));
        }
    }
}
