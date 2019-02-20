using System;

namespace BlazorGraphExample.Services
{
    public class TokenResult
    {
        public string IdToken { get; set; }
        public DateTime Expires { get; set; }
        public string AccountId { get; set; }
    }
}
