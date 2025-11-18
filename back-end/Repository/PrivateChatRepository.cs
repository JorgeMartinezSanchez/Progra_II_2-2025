using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.Interfaces;
using back_end.Models;
using MongoDB.Driver;

namespace back_end.Repository
{
    public class PrivateChatRepository : IPrivateChatRepository
    {
        private readonly IMongoCollection<PrivateChat> _privateChats;
        public PrivateChatRepository(IMongoDatabase database)
        {
            _privateChats = database.GetCollection<PrivateChat>("PrivateChat");
        }

        public async Task<PrivateChat> CreateAsync(PrivateChat privateChat)
        {
            await _privateChats.InsertOneAsync(privateChat);
            return privateChat;
        }

        public async Task<List<PrivateChat>> GetAllAsync()
        {
            return await _privateChats.Find(_ => true).ToListAsync();
        }

        public async Task<PrivateChat> GetByIdAsync(string id)
        {
            return await _privateChats.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<PrivateChat>> GetAllByAccountId(string _AccountId)
        {
            return await _privateChats.Find(a => a.Account1Id == _AccountId || a.Account2Id == _AccountId).ToListAsync();
        }
        public async Task<PrivateChat> GetByAccountId(string _AccountId)
        {
            return await _privateChats.Find(a => a.Account1Id == _AccountId || a.Account2Id == _AccountId).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _privateChats.DeleteOneAsync(a => a.Id == id);
        }
    }
}