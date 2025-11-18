

using back_end.Models;

namespace back_end.Interfaces
{
    public interface IMessageReposiroty
    {
        Task<Message> CreateAsync(Message message);          // ✅ CORRECCIÓN: Create en lugar de PostMessage
        Task UpdateAsync(string id, Message message);        // ✅ CORRECCIÓN: Update en lugar de Edit
        Task DeleteAsync(string id);
    }
}