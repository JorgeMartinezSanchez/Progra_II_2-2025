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
        private readonly IDesencrypteService _desencrypteService;

        public AccountService(IAccountRepository accountRepository, IDesencrypteService desencrypteService)
        {
            _accountRepository = accountRepository;
            _desencrypteService = desencrypteService;
        }
        
        public async Task<ReceiveAccountDto> LoginAsync(string username, string password)
        {
            var account = await _accountRepository.GetByUsernameAsync(username);
            if (account == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // Aquí necesitas la clave de encriptación - puedes obtenerla de configuración
            var encryptionKey = "tu-clave-secreta-de-encriptacion"; // Mover a appsettings
            
            if (!_desencrypteService.VerifyPassword(password, account.EncryptedPrivateKey, account.Salt, encryptionKey))
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            return MapToDto(account);
        }

        public async Task<List<ReceiveAccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(account => MapToDto(account)).ToList();
        }

        public async Task<ReceiveAccountDto> GetAccountByIdAsync(string id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {id} not found");

            return MapToDto(account);
        }
        public async Task<ReceiveAccountDto> CreateAccountAsync(CreateAccountDto createAccountDto)
        {
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

        public ReceiveAccountDto MapToDto(Account account)
        {
            return new ReceiveAccountDto
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