using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Models;

namespace back_end.Interfaces
{
    public interface IMessageService
    {
        Task<ReceiveMessageDto> SendMessageAsync(CreateMessageDto messageDto);
        Task<List<ReceiveMessageDto>> GetMessagesByPrivateChatAsync(string privateChatId);
        Task MarkAsSeen(string privateChatId);
        Task DeleteMessageAsync(ReceiveMessageDto message);
        Task DeleteManyMessagesAsync(List<ReceiveMessageDto> messages);
        ReceiveMessageDto MapToDto(Message message);
    }
}