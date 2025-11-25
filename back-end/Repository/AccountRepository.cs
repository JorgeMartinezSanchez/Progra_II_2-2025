using MongoDB.Driver;
using back_end.Models;
using back_end.Interfaces;

namespace back_end.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountRepository(IMongoDatabase database)
        {
            _accounts = database.GetCollection<Account>("Account");
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _accounts.Find(_ => true).ToListAsync();
        }

        public async Task<Account> GetByIdAsync(string id)
        {
            return await _accounts.Find(a => a.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Account> GetByUsernameAsync(string username)
        {
            return await _accounts.Find(a => a.Username == username).FirstOrDefaultAsync();
        }

        public async Task<Account> CreateAsync(Account account)
        {
            await _accounts.InsertOneAsync(account);
            return account;
        }

        public async Task UpdateAsync(string id, Account account)
        {
            await _accounts.ReplaceOneAsync(a => a.Id == id, account);
        }

        public async Task DeleteAsync(string id)
        {
            await _accounts.DeleteOneAsync(a => a.Id == id);
        }
    }
}