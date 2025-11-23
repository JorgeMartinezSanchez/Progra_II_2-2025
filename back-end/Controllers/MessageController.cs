using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetAllChatMessages(string chatId)
        {
            try
            {
                var AllChats = await _messageService.GetMessagesByPrivateChatAsync(chatId);
                return Ok(AllChats);
            } catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(string id)
        {
            try{
                var message = await _messageService.GetMessageById(id);
                return Ok(message);
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

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto createMessageDto)
        {
            try
            {
                var message = await _messageService.SendMessageAsync(createMessageDto);                
                return CreatedAtAction(
                    nameof(GetMessageById), 
                    new { id = message.Id }, 
                    message
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpPut("mark-seen/{chatId}")]
        public async Task<IActionResult> MarkMessagesAsSeen(string chatId)
        {
            try
            {
                await _messageService.MarkAsSeen(chatId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMessages([FromBody] List<ReceiveMessageDto> messages)
        {
            try
            {
                if (messages == null || messages.Count == 0)
                {
                    return BadRequest(new { message = "No messages provided" });
                }

                if (messages.Count > 1)
                {
                    await _messageService.DeleteManyMessagesAsync(messages);
                }
                else
                {
                    await _messageService.DeleteMessageAsync(messages[0]);
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}