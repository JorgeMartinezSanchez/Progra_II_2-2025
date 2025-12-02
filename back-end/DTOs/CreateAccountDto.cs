using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    public class CreateAccountDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Username inválido")]
        public string Username { get; set; } = string.Empty;
        
        public string Base64Pfp { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}