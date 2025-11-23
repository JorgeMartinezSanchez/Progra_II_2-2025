using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Interfaces;
using back_end.Models;

namespace back_end.Services
{
    public class ChatKeyStoreService : IChatKeyStoreService
    {
        private readonly IChatKeyStoreRepository _chatKeyStoreRepository;

        public ChatKeyStoreService(IChatKeyStoreRepository chatKeyStoreRepository)
        {
            _chatKeyStoreRepository = chatKeyStoreRepository;
        }

        public async Task<ReceiveChatKeyStoreDto> GetChatKeyAsync(string userId, string chatId)
        {
            var keyStore = await _chatKeyStoreRepository.GetByUserAndChatAsync(userId, chatId);
            
            if (keyStore == null)
                throw new KeyNotFoundException($"Chat key not found for user {userId} and chat {chatId}");

            return MapToDto(keyStore);
        }

        public async Task<List<ReceiveChatKeyStoreDto>> GetAllUserChatKeysAsync(string userId)
        {
            var keyStores = await _chatKeyStoreRepository.GetAllByUserIdAsync(userId);
            
            var result = new List<ReceiveChatKeyStoreDto>();
            foreach (var keyStore in keyStores)
            {
                result.Add(MapToDto(keyStore));
            }
            
            return result;
        }

        public async Task DeleteChatKeysAsync(string chatId)
        {
            await _chatKeyStoreRepository.DeleteByChatIdAsync(chatId);
        }

        private ReceiveChatKeyStoreDto MapToDto(ChatKeyStore keyStore)
        {
            return new ReceiveChatKeyStoreDto
            {
                Id = keyStore.Id,
                UserId = keyStore.AccountId,
                ChatId = keyStore.ChatId,
                EncryptedChatKey = keyStore.EncryptedChatKey,
                CreatedAt = keyStore.CreatedAt
            };
        }
    }
}
