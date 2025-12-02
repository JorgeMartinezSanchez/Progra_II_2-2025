using back_end.Models;
using back_end.DTOs;

namespace back_end.Interfaces
{
    public interface IAccountService
    {
        Task<List<ReceiveAccountDto>> GetAllAccountsAsync();
        Task<ReceiveAccountDto> GetAccountByIdAsync(string id);
        Task<ReceiveAccountDto> CreateAccountAsync(CreateAccountDto createAccountDto);
        Task<ReceiveAccountDto> LoginAsync(LoginDto login);
        Task DeleteAccountAsync(string id);
        ReceiveAccountDto MapToDto(Account account);
    }
}