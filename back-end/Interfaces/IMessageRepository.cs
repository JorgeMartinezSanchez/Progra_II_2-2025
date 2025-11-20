

using back_end.Models;

namespace back_end.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message);
        Task UpdateAsync(string id, Message message);
        Task DeleteAsync(Message message);
        Task _DeleteManyAsync(List<Message> messages);
        Task<Message> GetByIdAsync(string id);
        Task<List<Message>> GetByChatIdAsync(string chatId);
    }
}