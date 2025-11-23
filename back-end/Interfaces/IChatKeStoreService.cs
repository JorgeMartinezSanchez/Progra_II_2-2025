using System.Threading.Tasks;
using back_end.DTOs;

namespace back_end.Interfaces
{
    public interface IChatKeyStoreService
    {
        Task<ReceiveChatKeyStoreDto> GetChatKeyAsync(string userId, string chatId);
        Task<List<ReceiveChatKeyStoreDto>> GetAllUserChatKeysAsync(string userId);
        Task DeleteChatKeysAsync(string chatId);
    }
}
