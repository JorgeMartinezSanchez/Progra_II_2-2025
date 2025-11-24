using back_end.Models;

namespace back_end.Interfaces
{
    public interface IChatKeyStoreRepository
    {
        Task<ChatKeyStore> CreateAsync(ChatKeyStore keyStore);
        Task<ChatKeyStore> GetByUserAndChatAsync(string userId, string chatId);
        Task<List<ChatKeyStore>> GetAllByUserIdAsync(string userId);
        Task DeleteByChatIdAsync(string chatId);
    }
}