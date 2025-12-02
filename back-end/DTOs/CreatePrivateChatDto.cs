using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    public class CreatePrivateChatDto
    {
        [Required]
        public string AccountId { get; set; } = string.Empty;

        [Required]
        public string SendingUsername { get; set; } = string.Empty;
    }
}