using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    public class CreateMessageDto
    {
        [Required]
        public string ChatId { get; set; } = string.Empty;

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string EncryptedContent { get; set; } = string.Empty;
        
        [Required]
        public string Iv { get; set; } = string.Empty;
    }
}