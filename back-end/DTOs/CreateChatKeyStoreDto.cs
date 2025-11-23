using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.DTOs
{
    public class CreateChatKeyStoreDto
    {
        [Required(ErrorMessage = "UserId is required")]
        [StringLength(24, MinimumLength = 24, ErrorMessage = "UserId must be 24 characters")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ChatId is required")]
        [StringLength(24, MinimumLength = 24, ErrorMessage = "ChatId must be 24 characters")]
        public string ChatId { get; set; } = string.Empty;

        [Required(ErrorMessage = "EncryptedChatKey is required")]
        [MinLength(16, ErrorMessage = "EncryptedChatKey must be at least 16 characters")]
        public string EncryptedChatKey { get; set; } = string.Empty;
    }
}