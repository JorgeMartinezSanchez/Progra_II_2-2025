using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using back_end.Models;
using back_end.Interfaces;
using back_end.DTOs;

namespace back_end.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<AccountResponseDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(account => MapToDto(account)).ToList();
        }

        public async Task<AccountResponseDto> GetAccountByIdAsync(string id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {id} not found");

            return MapToDto(account);
        }
        public async Task<AccountResponseDto> CreateAccountAsync(CreateAccountDto createAccountDto)
        {
            // ✅ SIMPLIFICADO - Sin generación de userId
            var existingAccount = await _accountRepository.GetByUsernameAsync(createAccountDto.Username);
            if (existingAccount != null)
                throw new InvalidOperationException("Username already exists");

            var account = new Account
            {
                Username = createAccountDto.Username,
                Base64Pfp = createAccountDto.Base64Pfp,
                PublicKey = createAccountDto.PublicKey,
                EncryptedPrivateKey = createAccountDto.EncryptedPrivateKey,
                Salt = createAccountDto.Salt,
                CreatedAt = DateTime.UtcNow
            };

            var createdAccount = await _accountRepository.CreateAsync(account);
            return MapToDto(createdAccount);
        }

        public async Task DeleteAccountAsync(string id)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
                throw new KeyNotFoundException($"Account with ID {id} not found");

            await _accountRepository.DeleteAsync(id);
        }

        public AccountResponseDto MapToDto(Account account)
        {
            return new AccountResponseDto
            {
                Id = account.Id,
                Username = account.Username,
                Base64Pfp = account.Base64Pfp,
                PublicKey = account.PublicKey,
                CreatedAt = account.CreatedAt
            };
        }
    }
}