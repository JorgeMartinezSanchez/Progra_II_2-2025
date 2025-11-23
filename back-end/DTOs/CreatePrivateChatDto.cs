using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    public class CreatePrivateChatDto
    {
        [Required(ErrorMessage = "AccountId is required")]
        [StringLength(24, MinimumLength = 24, ErrorMessage = "AccountId must be 24 characters")]
        public string AccountId { get; set; } = string.Empty;

        [Required(ErrorMessage = "SendingUsername is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers and underscores")]
        public string SendingUsername { get; set; } = string.Empty;

        // Clave AES del chat cifrada con TU clave pública RSA
        [Required(ErrorMessage = "EncryptedChatKeyForMe is required")]
        [MinLength(16, ErrorMessage = "EncryptedChatKeyForMe must be at least 16 characters")]
        public string EncryptedChatKeyForMe { get; set; } = string.Empty;

        // Clave AES del chat cifrada con la clave pública RSA del OTRO usuario
        [Required(ErrorMessage = "EncryptedChatKeyForThem is required")]
        [MinLength(16, ErrorMessage = "EncryptedChatKeyForThem must be at least 16 characters")]
        public string EncryptedChatKeyForThem { get; set; } = string.Empty;
    }
}