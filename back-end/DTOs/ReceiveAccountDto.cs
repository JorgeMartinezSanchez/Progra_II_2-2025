namespace back_end.DTOs
{
    public class ReceiveAccountDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Base64Pfp { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}