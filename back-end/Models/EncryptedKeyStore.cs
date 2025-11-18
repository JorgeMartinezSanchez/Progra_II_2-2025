using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back_end.Models
{
    public class EncryptedKeyStore
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("messageId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MessageId { get; set; } = string.Empty;
        
        [BsonElement("encryptedAesKey")]
        public string EncryptedAesKey { get; set; } = string.Empty; // Clave AES encriptada con RSA
        
        [BsonElement("recipientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecipientId { get; set; } = string.Empty;
    }
}