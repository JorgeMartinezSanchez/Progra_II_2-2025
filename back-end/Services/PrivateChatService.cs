using back_end.DTOs;
using back_end.Interfaces;
using back_end.Models;
using back_end.Exceptions;

namespace back_end.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IPrivateChatRepository _privateChatRepository;
        private readonly IChatKeyStoreRepository _chatKeyStoreRepository;
        private readonly IAccountService _accountService;
        private readonly IMessageService _messageService;
        private readonly IEncryptationService _encryptationService;

        public PrivateChatService(
            IPrivateChatRepository privateChatRepository, 
            IChatKeyStoreRepository chatKeyStoreRepository,
            IAccountService accountService,
            IMessageService messageService,
            IEncryptationService encryptationService)
        {
            _privateChatRepository = privateChatRepository;
            _chatKeyStoreRepository = chatKeyStoreRepository;
            _accountService = accountService;
            _messageService = messageService;
            _encryptationService = encryptationService;
        }
        
        public async Task<bool> AlreadyAddedContactAsync(string id, string sendingUsername)
        {
            var you = await _accountService.GetAccountByIdAsync(id);
            var allContacts = await _privateChatRepository.GetAllAsync();

            foreach (var contact in allContacts)
            {
                var account1 = await _accountService.GetAccountByIdAsync(contact.Account1Id);
                var account2 = await _accountService.GetAccountByIdAsync(contact.Account2Id);

                bool contactHasOneOfTheSendingUsernames = 
                    account1.Username == sendingUsername || account2.Username == sendingUsername;
                bool contactHasYourUsername = 
                    account1.Username == you.Username || account2.Username == you.Username;

                if (contactHasOneOfTheSendingUsernames && contactHasYourUsername)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<ReceivePrivateChatDto> CreatePrivateChatAsync(CreatePrivateChatDto newPrivateChatDto)
        {
            // 1. Verificar si ya existe chat
            if (await AlreadyAddedContactAsync(newPrivateChatDto.AccountId, newPrivateChatDto.SendingUsername))
                throw new InvalidOperationException("Chat already exists with this user.");

            // 2. Obtener mi información
            var myAccount = await _accountService.GetAccountByIdAsync(newPrivateChatDto.AccountId);
            
            // 3. Buscar al otro usuario
            var allAccounts = await _accountService.GetAllAccountsAsync();
            var targetAccount = allAccounts.FirstOrDefault(a => a.Username == newPrivateChatDto.SendingUsername);
            
            if (targetAccount == null) 
                throw new KeyNotFoundException("User doesn't exist.");

            // 4. Generar clave AES para el chat (256 bits)
            var chatAesKey = _encryptationService.GenerateRandomAesKey();

            // 5. Obtener claves públicas de ambos usuarios
            var myPublicKey = myAccount.PublicKey;
            var targetPublicKey = targetAccount.PublicKey;

            // 6. Cifrar la clave AES con RSA pública de cada usuario
            var encryptedForMe = _encryptationService.EncryptWithRsa(chatAesKey, myPublicKey);
            var encryptedForThem = _encryptationService.EncryptWithRsa(chatAesKey, targetPublicKey);

            // 7. Crear el chat (ordenar IDs para consistencia)
            string account1Id, account2Id;
            if (string.Compare(newPrivateChatDto.AccountId, targetAccount.Id) < 0)
            {
                account1Id = newPrivateChatDto.AccountId;
                account2Id = targetAccount.Id;
            }
            else
            {
                account1Id = targetAccount.Id;
                account2Id = newPrivateChatDto.AccountId;
            }

            var newPrivateChat = new PrivateChat
            {
                Account1Id = account1Id,
                Account2Id = account2Id,
                CreatedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            var createdChat = await _privateChatRepository.CreateAsync(newPrivateChat);
            
            // 8. Guardar claves cifradas para ambos usuarios en ChatKeyStore
            await _chatKeyStoreRepository.CreateAsync(new ChatKeyStore
            {
                AccountId = newPrivateChatDto.AccountId,
                ChatId = createdChat.Id,
                EncryptedChatKey = encryptedForMe,
                CreatedAt = DateTime.UtcNow
            });
            
            await _chatKeyStoreRepository.CreateAsync(new ChatKeyStore
            {
                AccountId = targetAccount.Id,
                ChatId = createdChat.Id,
                EncryptedChatKey = encryptedForThem,
                CreatedAt = DateTime.UtcNow
            });
            
            return await MapToDto(createdChat, newPrivateChatDto.AccountId);
        }

        public async Task DeleteChatAsync(string chatId)
        {
            // Verificar que el chat existe
            var chat = await _privateChatRepository.GetByIdAsync(chatId);
            if (chat == null)
                throw new KeyNotFoundException($"Chat with ID {chatId} not found");

            // Eliminar mensajes del chat
            var messages = await _messageService.GetMessagesByPrivateChatAsync(chatId);
            await _messageService.DeleteManyMessagesAsync(messages);
            
            // Eliminar claves del chat
            await _chatKeyStoreRepository.DeleteByChatIdAsync(chatId);
            
            // Eliminar el chat
            await _privateChatRepository.DeleteAsync(chatId);
        }

        public async Task<List<ReceivePrivateChatDto>> LoadChats(string accountId)
        {
            // Obtener todos los ChatKeyStore del usuario
            var userChatKeys = await _chatKeyStoreRepository.GetAllByUserIdAsync(accountId);
            
            var allChats = new List<ReceivePrivateChatDto>();
            
            foreach (var chatKey in userChatKeys)
            {
                var chat = await _privateChatRepository.GetByIdAsync(chatKey.ChatId);
                
                if (chat == null) continue;

                string contactId = chat.Account1Id == accountId ? chat.Account2Id : chat.Account1Id;

                var contactAccount = await _accountService.GetAccountByIdAsync(contactId);
                
                allChats.Add(new ReceivePrivateChatDto
                {
                    Id = chat.Id,
                    Account1Id = chat.Account1Id,
                    Account2Id = chat.Account2Id,
                    CreatedAt = chat.CreatedAt,
                    LastActivity = chat.LastActivity,
                    ContactId = contactAccount.Id,
                    ContactUsername = contactAccount.Username,
                    ContactBase64Pfp = contactAccount.Base64Pfp
                });
            }
            
            return allChats;
        }

        public async Task<ReceivePrivateChatDto> MapToDto(PrivateChat privateChat, string currentUserId)
        {
            string contactId = privateChat.Account1Id == currentUserId ? privateChat.Account2Id : privateChat.Account1Id;
            var contactAccount = await _accountService.GetAccountByIdAsync(contactId);
            
            return new ReceivePrivateChatDto
            {
                Id = privateChat.Id,
                Account1Id = privateChat.Account1Id,
                Account2Id = privateChat.Account2Id,
                CreatedAt = privateChat.CreatedAt,
                LastActivity = privateChat.LastActivity,
                ContactId = contactAccount.Id,
                ContactUsername = contactAccount.Username,
                ContactBase64Pfp = contactAccount.Base64Pfp
            };
        }

        private ReceivePrivateChatDto MapToDto(PrivateChat privateChat)
        {            
            return new ReceivePrivateChatDto
            {
                Id = privateChat.Id,
                Account1Id = privateChat.Account1Id,
                Account2Id = privateChat.Account2Id,
                CreatedAt = privateChat.CreatedAt,
                LastActivity = privateChat.LastActivity
            };
        }

        public async Task<ReceivePrivateChatDto> GetPrivateChatById(string id)
        {
            var chat = await _privateChatRepository.GetByIdAsync(id);
            if (chat == null)
                throw new KeyNotFoundException($"Chat with ID {id} not found");
                
            return MapToDto(chat);
        }

        // Método para obtener la clave AES del chat (cifrada con RSA) para un usuario
        public async Task<string> GetEncryptedChatKey(string userId, string chatId)
        {
            ChatKeyStore chatKeyStore = await _chatKeyStoreRepository.GetByUserAndChatAsync(userId, chatId);
            if (chatKeyStore == null)
                throw new KeyNotFoundException($"Chat key not found for user {userId} in chat {chatId}");
                
            return chatKeyStore.EncryptedChatKey;
        }

        // Método para actualizar última actividad
        public async Task UpdateLastActivity(string chatId)
        {
            PrivateChat chat = await _privateChatRepository.GetByIdAsync(chatId);
            if (chat == null)
                throw new KeyNotFoundException($"Chat with ID {chatId} not found");

            chat.LastActivity = DateTime.UtcNow;
            await _privateChatRepository.UpdateAsync(chatId, chat);
        }
    }
}