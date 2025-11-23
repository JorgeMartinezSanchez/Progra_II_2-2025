using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back_end.Models
{
    public class ChatKeyStore
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("accountId")]
        public string AccountId { get; set; } = string.Empty;
        [BsonElement("chatId")]
        public string ChatId { get; set; } = string.Empty;
        [BsonElement("encryptedChatKey")]
        // Clave AES del chat, cifrada con la clave pública RSA de este usuario
        public string EncryptedChatKey { get; set; } = string.Empty;
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}