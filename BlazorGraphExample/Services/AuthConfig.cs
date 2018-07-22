using System.Collections.Generic;

namespace BlazorGraphExample.Services
{
    public class AuthConfig
    {
        public AuthConfig(string clientId, string[] scopes, string redirectUri)
        {
            ClientId = clientId;
            Scopes = new List<string>(scopes);
            RedirectUri = redirectUri;
        }

        public string ClientId { get; private set; }
        public IReadOnlyList<string> Scopes { get; private set; }
        public string RedirectUri { get; private set; }
    }
}
