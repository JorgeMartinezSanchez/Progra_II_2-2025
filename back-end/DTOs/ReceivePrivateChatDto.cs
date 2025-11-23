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
        public string Account1Id { get; set; } = string.Empty;
        
        [Required]
        public string Account2Id { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        
        // Información del contacto (opcional, útil para UI)
        public string? ContactId { get; set; }
        public string? ContactUsername { get; set; }
        public string? ContactBase64Pfp { get; set; }
    }
}