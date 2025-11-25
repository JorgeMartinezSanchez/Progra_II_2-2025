namespace back_end.Interfaces
{
    public interface IDesencrypteService
    {
        string DesencryptePassword(string password);
        string DesencryptePassword(string encryptedPassword, string key, string salt);
        string DesencrypteWithAES(string encryptedText, string base64Key, string base64IV);
        string DesencrypteWithRSA(string encryptedText, string rsaPrivateKey);
        string DesencrypteChatMessage(string encryptedContent, string base64ChatKey, string base64IV);
        string DesencrypteChatKey(string encryptedChatKey, string rsaPrivateKey);
        bool VerifyPassword(string inputPassword, string storedEncryptedPassword, string salt, string encryptionKey);
    }
}