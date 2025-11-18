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

        [Required(ErrorMessage = "EncryptedChatKey is required")]
        [MinLength(16, ErrorMessage = "EncryptedChatKey must be at least 16 characters")]
        public string EncryptedChatKey { get; set; } = string.Empty;
    }
}