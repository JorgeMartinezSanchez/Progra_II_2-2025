using back_end.DTOs;
using back_end.Models;

namespace back_end.Interfaces
{
    public interface IPrivateChatService
    {
        Task<ReceivePrivateChatDto> CreatePrivateChatAsync(CreatePrivateChatDto newPrivateChat);
        Task<bool> AlreadyAddedContactAsync(string id, string SendingUsername);
        Task DeleteChatAsync(string _id);
        Task<ReceivePrivateChatDto> GetPrivateChatById(string id);
        Task<List<ReceivePrivateChatDto>> LoadChats(string accountId);
        Task<ReceivePrivateChatDto> MapToDto(PrivateChat privateChat, string _id);
    }
}