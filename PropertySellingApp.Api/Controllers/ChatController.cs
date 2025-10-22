using Microsoft.AspNetCore.Mvc;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Services.Interfaces;

namespace PropertySellingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskGroq([FromBody] ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message cannot be empty.");

            var response = await _chatService.GetGroqReplyAsync(request);
            return Ok(response);
        }
    }
}
