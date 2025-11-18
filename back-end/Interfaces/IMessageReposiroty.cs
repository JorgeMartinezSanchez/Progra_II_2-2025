

using back_end.Models;

namespace back_end.Interfaces
{
    public interface IMessageReposiroty
    {
        Task<Message> CreateAsync(Message message);
        Task UpdateAsync(string id, Message message);
        Task DeleteAsync(string id);
        Task<Message> GetByIdAsync(string id);
        Task<List<Message>> GetByChatIdAsync(string chatId);
    }
}