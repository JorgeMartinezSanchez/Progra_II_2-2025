using System.Security.Cryptography;
using back_end.DTOs;
using back_end.Exceptions;
using back_end.Interfaces;
using back_end.Models;

namespace back_end.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageReposiroty;
        private readonly IEncryptationService _encryptationService;
        private readonly IChatKeyStoreService _chatKeyStoreService;
        public MessageService(IMessageRepository messageReposiroty, 
                              IEncryptationService encryptationService,
                              IChatKeyStoreService chatKeyStoreService)
        {
            _messageReposiroty = messageReposiroty;
            _encryptationService = encryptationService;
            _chatKeyStoreService = chatKeyStoreService;
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
            if (string.IsNullOrEmpty(messageDto.Content))
            {
                throw new EncryptionException("Message is Empty.");
            }

            try
            {
                // 1. Obtener la clave del chat para el usuario que envía el mensaje
                var chatKeyDto = await _chatKeyStoreService.GetChatKeyAsync(messageDto.SenderId, messageDto.ChatId);
                
                // 2. Desencriptar la clave AES del chat usando la clave privada del remitente
                string chatAesKey = _encryptationService.DecryptWithRsa(
                    chatKeyDto.EncryptedChatKey, 
                    messageDto.SenderPrivateKey
                );
                
                // 3. Encriptar el contenido del mensaje con AES usando la clave del chat
                (string encryptedContent, string iv) = _encryptationService.EncryptWithAes(
                    messageDto.Content, 
                    chatAesKey
                );
                
                // 4. Crear el mensaje para la base de datos
                var msgToDb = new Message
                {
                    ChatId = messageDto.ChatId,
                    SenderId = messageDto.SenderId,
                    EncryptedContent = encryptedContent,
                    Iv = iv,
                    TimeStamp = DateTime.UtcNow,
                    Status = "Sent"
                };

                // 5. Guardar el mensaje en la base de datos
                var sendMsg = await _messageReposiroty.CreateAsync(msgToDb);

                return MapToDto(sendMsg);
            }
            catch (KeyNotFoundException ex)
            {
                throw new EncryptionException($"Chat key not found for user {messageDto.SenderId} in chat {messageDto.ChatId}. Please ensure the chat is properly initialized.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new EncryptionException("Failed to encrypt message. Please check the encryption keys.", ex);
            }
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