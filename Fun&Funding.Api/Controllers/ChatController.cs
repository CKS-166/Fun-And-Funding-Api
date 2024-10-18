using Fun_Funding.Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("conversation/{senderId}/{receiverId}")]
        public async Task<IActionResult> GetChatConversation([FromRoute] Guid senderId,
            [FromRoute] Guid receiverId)
        {
            var response = await _chatService.GetChatConversation(senderId, receiverId);
            return Ok(response);
        }
    }
}
