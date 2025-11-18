using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.Models;

namespace back_end.Interfaces
{
    public interface IPrivateChatRepository
    {
        Task<List<PrivateChat>> GetAllAsync();
        Task<PrivateChat> GetByIdAsync(string id);
        Task<List<PrivateChat>> GetAllByAccountId(string _AccountId);
        Task<PrivateChat> GetByAccountId(string _AccountId);
        Task DeleteAsync(string id);
        Task<PrivateChat> CreateAsync(PrivateChat privateChat);
    }
}