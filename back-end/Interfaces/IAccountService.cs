// Interfaces/IAccountService.cs
using back_end.Models;
using back_end.DTOs;

namespace back_end.Interfaces
{
    public interface IAccountService
    {
        Task<List<AccountResponseDto>> GetAllAccountsAsync();
        Task<AccountResponseDto> GetAccountByIdAsync(string id);
        Task<AccountResponseDto> CreateAccountAsync(CreateAccountDto createAccountDto);
        Task DeleteAccountAsync(string id);
        AccountResponseDto MapToDto(Account account);
    }
}