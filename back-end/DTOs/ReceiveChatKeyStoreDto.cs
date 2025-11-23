using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.DTOs
{
    public class ReceiveChatKeyStoreDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ChatId { get; set; } = string.Empty;

        [Required]
        public string EncryptedChatKey { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}