using System.Security.Cryptography;
using System.Text;
using back_end.Interfaces;

namespace back_end.Services
{
    public class DesencrypteService : IDesencrypteService
    {
        // Método para desencriptar contraseña (ya existente)
        public string DesencryptePassword(string encryptedPassword, string key, string salt)
        {
            try
            {
                // Convertir la sal y la clave a bytes
                byte[] saltBytes = Convert.FromBase64String(salt);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                
                // Derivar la clave usando PBKDF2
                using var deriveBytes = new Rfc2898DeriveBytes(keyBytes, saltBytes, 10000, HashAlgorithmName.SHA256);
                byte[] derivedKey = deriveBytes.GetBytes(32); // 256 bits
                byte[] derivedIV = deriveBytes.GetBytes(16);  // 128 bits

                // Convertir el password encriptado a bytes
                byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);

                // Desencriptar usando AES
                using Aes aes = Aes.Create();
                aes.Key = derivedKey;
                aes.IV = derivedIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using MemoryStream memoryStream = new MemoryStream(encryptedBytes);
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using StreamReader streamReader = new StreamReader(cryptoStream);
                
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desencriptar la contraseña: {ex.Message}");
            }
        }

        // Método simple para desencriptar con AES (nuevo)
        public string DesencrypteWithAES(string encryptedText, string base64Key, string base64IV)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] key = Convert.FromBase64String(base64Key);
                byte[] iv = Convert.FromBase64String(base64IV);

                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using MemoryStream memoryStream = new MemoryStream(encryptedBytes);
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using StreamReader streamReader = new StreamReader(cryptoStream);
                
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desencriptar con AES: {ex.Message}");
            }
        }

        // Método simple para desencriptar con RSA (nuevo)
        public string DesencrypteWithRSA(string encryptedText, string rsaPrivateKey)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                
                using RSA rsa = RSA.Create();
                rsa.ImportFromPem(rsaPrivateKey); // Importar clave privada

                byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desencriptar con RSA: {ex.Message}");
            }
        }

        // Método para desencriptar mensajes de chat (usa AES)
        public string DesencrypteChatMessage(string encryptedContent, string base64ChatKey, string base64IV)
        {
            return DesencrypteWithAES(encryptedContent, base64ChatKey, base64IV);
        }

        // Método para desencriptar claves de chat (usa RSA)
        public string DesencrypteChatKey(string encryptedChatKey, string rsaPrivateKey)
        {
            return DesencrypteWithRSA(encryptedChatKey, rsaPrivateKey);
        }

        // Método simple (implementación requerida por la interfaz)
        public string DesencryptePassword(string password)
        {
            throw new NotImplementedException("Usa el otro método con key y salt");
        }

        // Verificación de contraseña (ya existente)
        public bool VerifyPassword(string inputPassword, string storedEncryptedPassword, string salt, string encryptionKey)
        {
            try
            {
                string decryptedPassword = DesencryptePassword(storedEncryptedPassword, encryptionKey, salt);
                return inputPassword == decryptedPassword;
            }
            catch
            {
                return false;
            }
        }
    }
}