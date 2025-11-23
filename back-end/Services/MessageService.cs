using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Exceptions;
using back_end.Interfaces;
using back_end.Models;

namespace back_end.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageReposiroty;
        public MessageService(IMessageRepository messageReposiroty)
        {
            _messageReposiroty = messageReposiroty;
        }

        public ReceiveMessageDto MapToDto(Message message)
        {
            return new ReceiveMessageDto
            {
              Id = message.Id,
              ChatId = message.ChatId,
              SenderId = message.SenderId,
              EncryptedContent = message.EncryptedContent,
              Iv = message.Iv,
              TimeStamp = message.TimeStamp,
              Status = message.Status
            };
        }
        public async Task<ReceiveMessageDto> GetMessageById(string id)
        {
            var message = await _messageReposiroty.GetByIdAsync(id);
            if (message == null)
                throw new KeyNotFoundException($"Account with ID {id} not found");

            return MapToDto(message);
        }
        public async Task<ReceiveMessageDto> SendMessageAsync(CreateMessageDto messageDto)
        {
            if (string.IsNullOrEmpty(messageDto.EncryptedContent))
            {
                throw new EncryptionException("Message is Empty.");
            }

            var MsgToDb = new Message
            {
                ChatId = messageDto.ChatId,
                SenderId = messageDto.SenderId,
                EncryptedContent = messageDto.EncryptedContent,
                Iv = messageDto.Iv,
                TimeStamp = DateTime.UtcNow,
                Status = "Sent"
            };

            var sendMsg = await _messageReposiroty.CreateAsync(MsgToDb);

            return MapToDto(sendMsg);
        }
        public async Task<List<ReceiveMessageDto>> GetMessagesByPrivateChatAsync(string privateChatId)
        {
            var allUnmappedMessages = await _messageReposiroty.GetByChatIdAsync(privateChatId);
            List<ReceiveMessageDto> result = new List<ReceiveMessageDto>();
            foreach(var UnmappedMessage in allUnmappedMessages)
            {
                result.Add(MapToDto(UnmappedMessage));
            }
            return result;
        }
        public async Task MarkAsSeen(string privateChatId)
        {
            var ChatMessages = await _messageReposiroty.GetByChatIdAsync(privateChatId);
            foreach(var Message in ChatMessages)
            {
                var SeenMessage = new Message
                {
                    Id = Message.Id,
                    ChatId = Message.ChatId,
                    SenderId = Message.SenderId,
                    EncryptedContent = Message.EncryptedContent,
                    Iv = Message.Iv,
                    TimeStamp = Message.TimeStamp,
                    Status = "Seen"
                };

                await _messageReposiroty.UpdateAsync(SeenMessage.Id, SeenMessage);
            }
        }
        public async Task DeleteMessageAsync(ReceiveMessageDto message)
        {
            await _messageReposiroty.DeleteAsync(new Message
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                EncryptedContent = message.EncryptedContent,
                Iv = message.Iv,
                TimeStamp = message.TimeStamp,
                Status = message.Status
            });
        }
        public async Task DeleteManyMessagesAsync(List<ReceiveMessageDto> messages)
        {
            List<Message> DtoToMapMessages = new List<Message>();
            foreach(ReceiveMessageDto message in messages)
            {
                DtoToMapMessages.Add(new Message
                {
                    Id = message.Id,
                    ChatId = message.ChatId,
                    SenderId = message.SenderId,
                    EncryptedContent = message.EncryptedContent,
                    Iv = message.Iv,
                    TimeStamp = message.TimeStamp,
                    Status = message.Status
                });
            }
            await _messageReposiroty._DeleteManyAsync(DtoToMapMessages);
        }
    }
}