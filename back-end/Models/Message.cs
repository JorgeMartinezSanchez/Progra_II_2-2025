using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back_end.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("chatId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ChatId { get; set; } = string.Empty;

        [BsonElement("senderId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; } = string.Empty;

        [BsonElement("encryptedContent")]
        public string EncryptedContent { get; set; } = string.Empty;
        [BsonElement("iv")]
        public string Iv { get; set; } = string.Empty;

        [BsonElement("timeStamp")]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;
    }
}