using back_end.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatKeyStoreController : ControllerBase
    {
        private readonly IChatKeyStoreService _chatKeyStoreService;

        public ChatKeyStoreController(IChatKeyStoreService chatKeyStoreService)
        {
            _chatKeyStoreService = chatKeyStoreService;
        }

        [HttpGet("user/{userId}/chat/{chatId}")]
        public async Task<IActionResult> GetChatKey(string userId, string chatId)
        {
            try
            {
                var chatKey = await _chatKeyStoreService.GetChatKeyAsync(userId, chatId);
                return Ok(chatKey);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllUserChatKeys(string userId)
        {
            try
            {
                var chatKeys = await _chatKeyStoreService.GetAllUserChatKeysAsync(userId);
                return Ok(chatKeys);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}