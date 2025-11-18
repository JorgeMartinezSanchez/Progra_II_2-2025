
namespace back_end.DTOs
{
    public class ReceiveMessageDto
    {
        public string Id { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string EncryptedContent { get; set; } = string.Empty;
        public string Iv { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}