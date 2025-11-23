using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back_end.Models
{
    public class PrivateChat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("accountId1")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Account1Id { get; set; } = string.Empty;

        [BsonElement("accountId2")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Account2Id { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("lastActivity")]
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }
}