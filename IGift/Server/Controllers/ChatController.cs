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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoadChatUsers()
        {
            var idCurrentUser = "32277405-7040-4b5a-b6f7-7bae3142e5c9";
            return Ok(await _chatService.LoadChatUsers(idCurrentUser));
        }
    }
}
