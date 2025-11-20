using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.Interfaces;
using back_end.Models;
using MongoDB.Driver;

namespace back_end.Repository
{
    public class MessageRepository : IMessageRepository
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
        public async Task UpdateAsync(string messageId, Message message)
        {
            await _messages.ReplaceOneAsync(a => a.Id == messageId, message);
        }
        public async Task DeleteAsync(Message message)
        {
            await _messages.DeleteOneAsync(a => a.Id == message.Id);
        }
        public async Task _DeleteManyAsync(List<Message> messages)
        {
            var messageIds = messages.Select(m => m.Id).ToList();
            
            var filter = Builders<Message>.Filter.In(m => m.Id, messageIds);
            await _messages.DeleteManyAsync(filter);
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