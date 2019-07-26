using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;
using W8lessLabs.GraphAPI;

namespace BlazorGraphExample.Services
{
    public class AuthService : IAuthTokenProvider
    {
        private class MSALAuthConfig
        {
            public string ClientId { get; set; }
            public string[] Scopes { get; set; }
        }

        private readonly MSALAuthConfig _config;
        private IJSInProcessRuntime _jsRuntime;

        public AuthService(AuthConfig config, IJSRuntime jsRuntime)
        {
            _config = new MSALAuthConfig()
            {
                ClientId = config.ClientId,
                Scopes = config.Scopes.ToArray()
            };
            _jsRuntime = (IJSInProcessRuntime)jsRuntime;
        }

        public async Task<GraphAccount> GetUserAccountAsync() =>
            await _jsRuntime.InvokeAsync<GraphAccount>("getUserAccount", _config);

        public bool IsLoggedIn(GraphAccount account) =>
            (account != null && account.Expires > DateTime.Now);

        public async Task<(GraphTokenResult token, GraphAccount account)> LoginAsync()
        {
            var tokenResult = await GetTokenAsync();
            return (tokenResult, await GetUserAccountAsync());
        }

        public async Task<bool> LogoutAsync() =>
            await _jsRuntime.InvokeAsync<bool>("logout", _config);

        public async Task<GraphTokenResult> GetTokenAsync(string accountId = null, bool forceRefresh = false)
        {
            try
            {
                var result = await _jsRuntime.InvokeAsync<TokenResult>("getTokenAsync", _config);
                return new GraphTokenResult(result?.IdToken != null, result.IdToken, result.Expires);
            }
            catch (Exception)
            {
                return GraphTokenResult.Failed;
            }
        }
    }
}
