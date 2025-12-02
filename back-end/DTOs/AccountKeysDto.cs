using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.DTOs
{
    public class AccountKeysDto
    {
        public string PublicKey { get; set; } = string.Empty;
        public string EncryptedPrivateKey { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}