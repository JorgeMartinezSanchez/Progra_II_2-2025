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
        Task<ReceiveUnencryptedMessageDto> UnencrypteMessage(ReceiveMessageDto encryptedMessage);
        Task<ReceiveMessageDto> SendMessageAsync(CreateMessageDto messageDto);
        Task<List<ReceiveMessageDto>> GetMessagesByPrivateChatAsync(string privateChatId);
        Task DeleteMessageAsync(string messageId);
        Task UpdateMessageStatusAsync(string messageId, string status);
        Task<ReceiveMessageDto> MapToDto(Message message);
    }
}