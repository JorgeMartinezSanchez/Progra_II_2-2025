using back_end.DTOs;
using back_end.Interfaces;
using back_end.Models;

namespace back_end.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IPrivateChatRepository _privateChatRepository;
        private readonly IChatKeyStoreRepository _chatKeyStoreRepository;
        private readonly IAccountService _accountService;
        private readonly IMessageService _messageService;

        public PrivateChatService(
            IPrivateChatRepository privateChatRepository, 
            IChatKeyStoreRepository chatKeyStoreRepository,
            IAccountService accountService,
            IMessageService messageService)
        {
            _privateChatRepository = privateChatRepository;
            _chatKeyStoreRepository = chatKeyStoreRepository;
            _accountService = accountService;
            _messageService = messageService;
        }
        
        public async Task<bool> AlreadyAddedContactAsync(string id, string SendingUsername)
        {
            var You = await _accountService.GetAccountByIdAsync(id);
            var AllContacts = await _privateChatRepository.GetAllAsync();

            foreach (var Contact in AllContacts)
            {
                var Account1 = await _accountService.GetAccountByIdAsync(Contact.Account1Id);
                var Account2 = await _accountService.GetAccountByIdAsync(Contact.Account2Id);

                bool ContactHasOneOfTheSendingUsernames = Account1.Username == SendingUsername || Account2.Username == SendingUsername;
                bool ContactHasYourUsername = Account1.Username == You.Username || Account2.Username == You.Username;

                if(ContactHasOneOfTheSendingUsernames && ContactHasYourUsername)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<ReceivePrivateChatDto> CreatePrivateChatAsync(CreatePrivateChatDto newPrivateChatParam)
        {
            if (string.IsNullOrWhiteSpace(newPrivateChatParam.EncryptedChatKeyForMe)) 
                throw new ArgumentException("Encrypted chat key for me is required");

            if (string.IsNullOrWhiteSpace(newPrivateChatParam.EncryptedChatKeyForThem)) 
                throw new ArgumentException("Encrypted chat key for them is required");

            if (!await AlreadyAddedContactAsync(newPrivateChatParam.AccountId, newPrivateChatParam.SendingUsername))
            {
                var allAccounts = await _accountService.GetAllAccountsAsync();
                var targetAccount = allAccounts.FirstOrDefault(a => a.Username == newPrivateChatParam.SendingUsername);
                
                if (targetAccount == null) throw new KeyNotFoundException("User doesn't exist.");

                string account1Id, account2Id;
                if (string.Compare(newPrivateChatParam.AccountId, targetAccount.Id) < 0)
                {
                    account1Id = newPrivateChatParam.AccountId;
                    account2Id = targetAccount.Id;
                }
                else
                {
                    account1Id = targetAccount.Id;
                    account2Id = newPrivateChatParam.AccountId;
                }

                var newPrivateChat = new PrivateChat
                {
                    Account1Id = account1Id,
                    Account2Id = account2Id,
                    CreatedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow
                };

                var createdChat = await _privateChatRepository.CreateAsync(newPrivateChat);
                
                // Guardar clave para el usuario actual
                await _chatKeyStoreRepository.CreateAsync(new ChatKeyStore
                {
                    AccountId = newPrivateChatParam.AccountId,
                    ChatId = createdChat.Id,
                    EncryptedChatKey = newPrivateChatParam.EncryptedChatKeyForMe,
                    CreatedAt = DateTime.UtcNow
                });
                
                // Guardar clave para el otro usuario
                await _chatKeyStoreRepository.CreateAsync(new ChatKeyStore
                {
                    AccountId = targetAccount.Id,
                    ChatId = createdChat.Id,
                    EncryptedChatKey = newPrivateChatParam.EncryptedChatKeyForThem,
                    CreatedAt = DateTime.UtcNow
                });
                
                return await MapToDto(createdChat, newPrivateChatParam.AccountId);
            }
            else
            {
                throw new InvalidOperationException("Chat already exists with this user.");
            }
        }

        public async Task DeleteChatAsync(string chatId)
        {
            List<ReceiveMessageDto> messages = await _messageService.GetMessagesByPrivateChatAsync(chatId);
            await _messageService.DeleteManyMessagesAsync(messages);
            await _chatKeyStoreRepository.DeleteByChatIdAsync(chatId);
            await _privateChatRepository.DeleteAsync(chatId);
        }
        public async Task<List<ReceivePrivateChatDto>> LoadChats(string accountId)
        {
            // ✅ Obtener todos los ChatKeyStore del usuario (1 query)
            var userChatKeys = await _chatKeyStoreRepository.GetAllByUserIdAsync(accountId);
            
            List<ReceivePrivateChatDto> allChats = new List<ReceivePrivateChatDto>();
            
            foreach(var chatKey in userChatKeys)
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
            return MapToDto(await _privateChatRepository.GetByIdAsync(id));
        }
    }
}