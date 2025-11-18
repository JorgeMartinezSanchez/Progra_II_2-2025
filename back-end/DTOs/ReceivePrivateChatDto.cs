using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.DTOs
{
    public class ReceivePrivateChatDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        [Required]
        public string ChatId { get; set; } = string.Empty;
        [Required]
        public string SenderId { get; set; } = string.Empty;
        [Required]
        public string EncryptedContent { get; set; } = string.Empty;
        [Required]
        public string Iv { get; set; } = string.Empty;
        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}