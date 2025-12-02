using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using back_end.Models;
using back_end.Interfaces;
using back_end.DTOs;
using System.Security.Cryptography;

namespace back_end.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEncryptationService _encryptationService;

        public AccountService(IAccountRepository accountRepository, IEncryptationService encryptationService)
        {
            _accountRepository = accountRepository;
            _encryptationService = encryptationService;
        }
        
        public async Task<ReceiveAccountDto> LoginAsync(LoginDto login)
        {
            var account = await _accountRepository.GetByUsernameAsync(login.Username);
            if (account == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            try
            {
                // 1. Derivar la clave AES del password (igual que en CreateAccountAsync)
                var derivedKey = _encryptationService.DeriveKeyFromPassword(
                    login.Password, 
                    account.Salt
                );

                // 2. Intentar descifrar la privateKey RSA
                // Si el password es correcto, esto funcionará
                var privateKey = _encryptationService.DecryptWithAes(
                    account.EncryptedPrivateKey,  // PrivateKey cifrada
                    account.EncryptionIV,         // ¡Necesita el IV!
                    derivedKey                    // Clave derivada del password
                );

                // 3. Si llegamos aquí, el password es CORRECTO
                // (opcional) Verificar que la privateKey descifrada es válida
                if (string.IsNullOrEmpty(privateKey))
                    throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

                return MapToDto(account);
            }
            catch (CryptographicException)
            {
                // Error de cifrado = password incorrecto
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
            }
            catch (Exception)
            {
                // Cualquier otro error
                throw new UnauthorizedAccessException("Error en la autenticación");
            }
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
            Account existingAccount = await _accountRepository.GetByUsernameAsync(createAccountDto.Username);
            if (existingAccount != null) 
                throw new InvalidOperationException("Username already exists");

            // 1. Generar par de claves RSA
            var (user_public_k, user_private_k) = _encryptationService.GenerateRsaKeyPair();

            // 2. Generar salt para el password
            var salt = _encryptationService.GenerateSalt();

            // 3. Derivar clave AES del password (para cifrar la privateKey RSA)
            var derivedKey = _encryptationService.DeriveKeyFromPassword(
                createAccountDto.Password, 
                salt
            );

            // 4. Cifrar la clave privada RSA con la clave derivada del password
            var (encryptedPrivateKey, encryptionIv) = _encryptationService.EncryptWithAes(
                user_private_k,  // Clave privada RSA
                derivedKey       // Clave derivada del password
            );

            // 5. Crear la cuenta con todos los datos cifrados
            var newAccount = new Account
            {
                Username = createAccountDto.Username,
                Base64Pfp = createAccountDto.Base64Pfp,
                PublicKey = user_public_k,          // RSA pública (NO cifrada)
                EncryptedPrivateKey = encryptedPrivateKey, // RSA privada cifrada con AES
                Salt = salt,                        // Para derivar clave después
                EncryptionIV = encryptionIv,         // ¡IMPORTANTE! Para descifrar después
                CreatedAt = DateTime.UtcNow
            };

            var createdAccount = await _accountRepository.CreateAsync(newAccount);
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
                Username  = account.Username,
                Base64Pfp = account.Base64Pfp,
                PublicKey  = account.PublicKey,
                EncryptedPrivateKey = account.EncryptedPrivateKey,
                Salt = account.Salt,
                EncryptionIV = account.EncryptionIV,
                CreatedAt = account.CreatedAt
            };
        }
    }
}