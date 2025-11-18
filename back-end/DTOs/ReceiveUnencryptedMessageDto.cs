using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.DTOs
{
    public class ReceiveUnencryptedMessageDto
    {
        public string Id { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}