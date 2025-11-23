using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp.Unsafe;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrivateChatController : ControllerBase
    {
        private readonly IPrivateChatService _privateChatService;

        public PrivateChatController(IPrivateChatService privateChatService)
        {
            _privateChatService = privateChatService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(CreatePrivateChatDto newChat)
        {
            try
            {
                var createdChat = await _privateChatService.CreatePrivateChatAsync(newChat);
                return CreatedAtAction(nameof(GetPrivateChatById), new { id = createdChat.Id }, createdChat);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("one/{id}")]
        public async Task<IActionResult> GetPrivateChatById(string id)
        {
            try{
                var Chat = await _privateChatService.GetPrivateChatById(id);
                return Ok(Chat);
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

        [HttpGet("all/{CurrentAccountId}")]
        public async Task<IActionResult> GetAllContactsFromOneAccount(string CurrentAccountId)
        {
            try
            {
                var contacts = await _privateChatService.LoadChats(CurrentAccountId);
                return Ok(contacts);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(string id)
        {
            try
            {
                await _privateChatService.DeleteChatAsync(id);
                return NoContent();
            } catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}