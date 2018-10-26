using System;

namespace BlazorGraphExample.Services
{
    public class GetTokenResult
    {
        public string IdToken { get; set; }
        public DateTime Expires { get; set; }
    }
}
