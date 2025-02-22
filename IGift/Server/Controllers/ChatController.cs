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
        public async Task<ActionResult> LoadChatUsers()
        {
            var idCurrentUser = "32277405-7040-4b5a-b6f7-7bae3142e5c9";
            return Ok(await _chatService.LoadChatUsers(idCurrentUser));
        }

        [HttpPost("GetChatById")] // Ruta personalizada
        [AllowAnonymous]
        public async Task<ActionResult> GetChatById()
        {
            var idCurrentUser = "97476a3e-c2e0-4e0a-9eff-5b2c69e37483";
            return Ok(await _chatService.GetChatHistoryByIdAsync(idCurrentUser));
        }
    }
}
