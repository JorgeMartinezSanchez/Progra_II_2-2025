using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    // CAMBIAR de EncryptedChatKey a ChatKey (simple)
    public class ReceiveChatKeyStoreDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ChatId { get; set; } = string.Empty;
        [Required]
        public string ChatKey { get; set; } = string.Empty;
        public string EncryptedChatKey { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}