using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.Interfaces;
using back_end.Models;
using MongoDB.Driver;

namespace back_end.Repository
{
    public class ChatKeyStoreRepository : IChatKeyStoreRepository
    {
        private readonly IMongoCollection<ChatKeyStore> _keyStore;

        public ChatKeyStoreRepository(IMongoDatabase database)
        {
            _keyStore = database.GetCollection<ChatKeyStore>("ChatKeyStore");
        }

        public async Task<ChatKeyStore> CreateAsync(ChatKeyStore keyStore)
        {
            await _keyStore.InsertOneAsync(keyStore);
            return keyStore;
        }

        public async Task<ChatKeyStore> GetByUserAndChatAsync(string userId, string chatId)
        {
            return await _keyStore.Find(k => k.AccountId == userId && k.ChatId == chatId)
                                .FirstOrDefaultAsync();
        }

        public async Task<List<ChatKeyStore>> GetAllByUserIdAsync(string userId)
        {
            return await _keyStore.Find(k => k.AccountId == userId)
                                .ToListAsync();
        }

        public async Task DeleteByChatIdAsync(string chatId)
        {
            await _keyStore.DeleteManyAsync(k => k.ChatId == chatId);
        }
    }
}