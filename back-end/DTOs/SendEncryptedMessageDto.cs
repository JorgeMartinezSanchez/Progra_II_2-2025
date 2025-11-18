using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    public class SendEncryptedMessageDto
    {
        [Required]
        public string ChatId { get; set; } = string.Empty;

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string EncryptedContent { get; set; } = string.Empty; // Contenido AES-cifrado

        [Required]
        public string Iv { get; set; } = string.Empty; // IV para AES

        [Required]
        public string EncryptedAesKey { get; set; } = string.Empty; // Clave AES RSA-cifrada
    }
    }
}