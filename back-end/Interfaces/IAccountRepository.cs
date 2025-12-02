using back_end.Models;

namespace back_end.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account> GetByIdAsync(string id);
        Task<Account> CreateAsync(Account account);
        Task<Account> UpdateAsync(string id, Account account);
        Task DeleteAsync(string id);
        Task<Account> GetByUsernameAsync(string username);
    }
}