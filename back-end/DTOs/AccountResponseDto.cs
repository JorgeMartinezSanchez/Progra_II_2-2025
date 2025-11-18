using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// DTOs/AccountResponseDto.cs
namespace back_end.DTOs
{
    public class AccountResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Base64Pfp { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}