using back_end.DTOs;
using back_end.Interfaces;
using back_end.Models;
using Microsoft.IdentityModel.Tokens;

namespace back_end.Services
{
    public class PrivateChatService : IPrivateChatService{
        private readonly IPrivateChatRepository _privateChatRepository;
        private readonly IAccountService _accountService; // servicio de account

        public PrivateChatService(IPrivateChatRepository privateChatRepository, IAccountService accountService)
        {
            _privateChatRepository = privateChatRepository;
            _accountService = accountService;
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
            // Validación adicional
            if (string.IsNullOrWhiteSpace(newPrivateChatParam.EncryptedChatKey)) throw new ArgumentException("Encrypted chat key is required");

            if (!await AlreadyAddedContactAsync(newPrivateChatParam.AccountId, newPrivateChatParam.SendingUsername))
            {
                var allAccounts = await _accountService.GetAllAccountsAsync();
                var targetAccount = allAccounts.FirstOrDefault(a => a.Username == newPrivateChatParam.SendingUsername);
                
                if (targetAccount == null) throw new KeyNotFoundException("User doesn't exist.");

                // Determinar quién es account1 y account2 de manera consistente
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
                    ChatKey = newPrivateChatParam.EncryptedChatKey, // Clave cifrada
                    CreatedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow
                };

                // Guardar en base de datos
                var createdChat = await _privateChatRepository.CreateAsync(newPrivateChat);
                
                // Mapear a DTO con información enriquecida - pasar el ID del usuario actual
                return await MapToDto(createdChat, newPrivateChatParam.AccountId);
            }
            else
            {
                throw new InvalidOperationException("Chat already exists with this user.");
            }
        }
        public async Task DeleteChatAsync(string _id)
        {
            await _privateChatRepository.DeleteAsync(_id);
        }
        public async Task<List<ReceivePrivateChatDto>> LoadChats(string _id)
        {
            List<ReceivePrivateChatDto> allChats = new List<ReceivePrivateChatDto>();
            var AllContacts = await _privateChatRepository.GetAllByAccountId(_id);
            foreach(var Contact in AllContacts)
            {
                allChats.Add(await MapToDto(Contact, _id));
            }
            return allChats;
        }
        public async Task<ReceivePrivateChatDto> MapToDto(PrivateChat privateChat, string currentUserId)
        {
            // Determinar cuál es el ID del contacto (el que NO es el usuario actual)
            string contactId = privateChat.Account1Id == currentUserId ? privateChat.Account2Id : privateChat.Account1Id;
            
            // Obtener la información del contacto
            var contactAccount = await _accountService.GetAccountByIdAsync(contactId);
            
            return new ReceivePrivateChatDto
            {
                ReceiverAccountUsername = contactAccount.Username,
                ChatKey = privateChat.ChatKey,
                CreatedAt = privateChat.CreatedAt,
                LastActivity = privateChat.LastActivity
            };
        }
    }
}