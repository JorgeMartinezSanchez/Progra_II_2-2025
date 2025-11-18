using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.Interfaces;
using back_end.Models;
using MongoDB.Driver;

namespace back_end.Repository
{
    public class MessageRepository : IMessageReposiroty
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageRepository(IMongoDatabase database)
        {
            _messages = database.GetCollection<Message>("Message");
        }

        public async Task<Message> CreateAsync(Message message)
        {
            await _messages.InsertOneAsync(message);
            return message;
        }
        public async Task UpdateAsync(string id, Message message)
        {
            await _messages.ReplaceOneAsync(a => a.Id == id, message);
        }
        public async Task DeleteAsync(string id)
        {
            await _messages.DeleteOneAsync(a => a.Id == id);
        }
        public async Task<Message> GetByIdAsync(string id)
        {
            return await _messages.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Message>> GetByChatIdAsync(string chatId)
        {
            return await _messages.Find(m => m.ChatId == chatId)
                                .SortBy(m => m.TimeStamp)
                                .ToListAsync();
        }
    }
}