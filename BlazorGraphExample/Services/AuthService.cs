using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using W8lessLabs.GraphAPI;

namespace BlazorGraphExample.Services
{
    public class AuthService : IAuthTokenProvider
    {
        private readonly AuthConfig _config;

        public AuthService(AuthConfig config)
        {
            _config = config;
        }

        private IJSInProcessRuntime _JS => (IJSInProcessRuntime)JSRuntime.Current;

        public async Task<GraphAccount> GetUserAccountAsync() =>
            await _JS.InvokeAsync<GraphAccount>("getUserAccount", _config);

        public bool IsLoggedIn(GraphAccount account) =>
            (account != null && account.Expires > DateTime.Now);

        public async Task<(GraphTokenResult token, GraphAccount account)> LoginAsync()
        {
            var tokenResult = await GetTokenAsync();
            return (tokenResult, await GetUserAccountAsync());
        }

        public async Task<bool> LogoutAsync() =>
            await _JS.InvokeAsync<bool>("logout", _config);

        public async Task<GraphTokenResult> GetTokenAsync(string accountId = null, bool forceRefresh = false)
        {
            try
            {
                var result = await _JS.InvokeAsync<TokenResult>("getTokenAsync", _config);
                return new GraphTokenResult(result?.IdToken != null, result.IdToken, result.Expires);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return GraphTokenResult.Failed;
            }
        }
    }
}
