using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Interfaces;
using back_end.Models;

namespace back_end.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageReposiroty _messageReposiroty;
        public MessageService(IMessageReposiroty messageReposiroty)
        {
            _messageReposiroty = messageReposiroty;
        }

        public async Task<ReceiveMessageDto> MapToDto(Message message)
        {
            return new ReceiveMessageDto
            {
              ChatId = message.ChatId,
              SenderId = message.SenderId,
              EncryptedContent = message.EncryptedContent,
              Iv = message.Iv,
              TimeStamp = message.TimeStamp,
                
            };
        }
        public async Task<ReceiveMessageDto> SendMessageAsync(CreateMessageDto messageDto)
        {
            try
            {
                
            }
        }
    }
}