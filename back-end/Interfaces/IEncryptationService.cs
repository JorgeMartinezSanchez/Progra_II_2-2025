namespace back_end.Interfaces
{
    public interface IEncryptationService
    {
        // Generación de claves RSA
        (string publicKey, string privateKey) GenerateRsaKeyPair();
        string EncryptWithRsa(string plainText, string publicKey);
        string DecryptWithRsa(string encryptedText, string privateKey);
        
        // Derivación de clave a partir de password
        string DeriveKeyFromPassword(string password, string salt, int keySize = 32);
        string GenerateSalt(int size = 16);
        
        // Cifrado/Descifrado AES para mensajes
        (string encryptedContent, string iv) EncryptWithAes(string plainText, string aesKey);
        string DecryptWithAes(string encryptedContent, string iv, string aesKey);
        
        // Manejo de claves AES de chat
        string GenerateRandomAesKey(int size = 32);
        
        // Para el nuevo flujo (sin cifrado en backend)
        Task<string> GetPublicKeyFromAccount(string accountId);
        bool VerifyUserPassword(string password, string storedHash, string salt);
    }
}