using System.Collections.Generic;

namespace BlazorGraphExample.Services
{
    public class AuthConfig
    {
        public AuthConfig(string clientId, string[] scopes)
        {
            ClientId = clientId;
            Scopes = new List<string>(scopes);
        }

        public string ClientId { get; private set; }
        public IReadOnlyList<string> Scopes { get; private set; }
    }
}
