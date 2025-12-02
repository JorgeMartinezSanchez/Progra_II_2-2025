using System.Security.Cryptography;
using System.Text;
using back_end.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;
using back_end.Models;

namespace back_end.Services
{
    public class EncryptationService : IEncryptationService
    {
        private readonly int _rsaKeySize = 2048;
        private readonly int _aesKeySize = 256;
        private readonly int _saltSize = 16;
        private readonly int _iterations = 100000;
        private readonly IAccountRepository _accountRepository;

        public EncryptationService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // 1. Generación de par de claves RSA
        public (string publicKey, string privateKey) GenerateRsaKeyPair()
        {
            using var rsa = RSA.Create(_rsaKeySize);
            return (
                publicKey: rsa.ToXmlString(false),  // Solo clave pública
                privateKey: rsa.ToXmlString(true)   // Par completo (pública + privada)
            );
        }

        // 2. Cifrado con RSA (usando clave pública)
        public string EncryptWithRsa(string plainText, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(publicKey);
            
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(bytes, RSAEncryptionPadding.OaepSHA256);
            
            return Convert.ToBase64String(encryptedBytes);
        }

        // 3. Descifrado con RSA (usando clave privada)
        public string DecryptWithRsa(string encryptedText, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.FromXmlString(privateKey);
            
            var bytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // 4. Derivación de clave a partir de password (para cifrar clave privada)
        public string DeriveKeyFromPassword(string password, string salt, int keySize = 32)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var deriveBytes = new Rfc2898DeriveBytes(
                password, 
                saltBytes, 
                _iterations, 
                HashAlgorithmName.SHA256
            );
            
            var keyBytes = deriveBytes.GetBytes(keySize);
            return Convert.ToBase64String(keyBytes);
        }

        // 5. Generación de salt
        public string GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        // 6. Cifrado AES para mensajes
        public (string encryptedContent, string iv) EncryptWithAes(string plainText, string aesKey)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(aesKey);
            aes.GenerateIV();
            
            var iv = aes.IV;
            var encryptor = aes.CreateEncryptor(aes.Key, iv);
            
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            
            sw.Write(plainText);
            sw.Close();
            
            return (
                encryptedContent: Convert.ToBase64String(ms.ToArray()),
                iv: Convert.ToBase64String(iv)
            );
        }

        // 7. Descifrado AES para mensajes
        public string DecryptWithAes(string encryptedContent, string iv, string aesKey)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(aesKey);
            aes.IV = Convert.FromBase64String(iv);
            
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var bytes = Convert.FromBase64String(encryptedContent);
            
            using var ms = new MemoryStream(bytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }

        // 8. Generación de clave AES aleatoria
        public string GenerateRandomAesKey(int size = 32)
        {
            var key = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return Convert.ToBase64String(key);
        }

        // 9. Obtener clave pública de una cuenta (simulación)
        public async Task<string> GetPublicKeyFromAccount(string accountId)
        {
            Account acc = await _accountRepository.GetByIdAsync(accountId);
            return acc.PublicKey;
        }

        // 10. Verificar password del usuario
        public bool VerifyUserPassword(string password, string storedHash, string salt)
        {
            var derivedKey = DeriveKeyFromPassword(password, salt);
            return derivedKey == storedHash;
        }

        // Método adicional útil para tu flujo
        public (string encryptedForUser1, string encryptedForUser2) EncryptChatKeyForBothUsers(
            string chatAesKey, 
            string publicKeyUser1, 
            string publicKeyUser2)
        {
            return (
                encryptedForUser1: EncryptWithRsa(chatAesKey, publicKeyUser1),
                encryptedForUser2: EncryptWithRsa(chatAesKey, publicKeyUser2)
            );
        }
    }
}