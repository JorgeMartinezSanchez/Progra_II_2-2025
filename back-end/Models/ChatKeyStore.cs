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
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("chatId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ChatId { get; set; } = string.Empty;

        [BsonElement("encryptedChatKey")]
        public string EncryptedChatKey { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}