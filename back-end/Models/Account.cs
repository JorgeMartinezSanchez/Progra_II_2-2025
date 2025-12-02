using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back_end.Models
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("base64Pfp")]
        public string Base64Pfp { get; set; } = string.Empty;

        [BsonElement("publicKey")]
        public string PublicKey { get; set; } = string.Empty;

        [BsonElement("encryptedPrivateKey")]
        public string EncryptedPrivateKey { get; set; } = string.Empty;

        [BsonElement("salt")]
        public string Salt { get; set; } = string.Empty;

        [BsonElement("encryptionIV")]
        public string EncryptionIV { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}