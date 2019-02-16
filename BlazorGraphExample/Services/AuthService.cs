using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using W8lessLabs.GraphAPI;

namespace BlazorGraphExample.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthConfig _config;

        public AuthService(AuthConfig config)
        {
            _config = config;
        }

        private IJSInProcessRuntime _JS => (IJSInProcessRuntime)JSRuntime.Current;

        public async Task<GraphAccount[]> GetUserAccountsAsync()
        {
            var account = await _JS.InvokeAsync<GraphAccount>("getUserAccount", _config);
            if (account is null)
                return new GraphAccount[0];
            else
                return new GraphAccount[] { account };
        }

        public async Task<GraphAuthResponse> LoginAsync()
        {
            (bool success, GraphAuthResponse response) = await TryGetTokenAsync(null);
            if (success)
                return response;
            else
                return default;
        }

        public async Task<GraphAuthResponse> LoginAsync(GraphAccount account) =>
            await LoginAsync();

        public async Task<bool> LogoutAsync(GraphAccount account) =>
            await _JS.InvokeAsync<bool>("logout", _config);

        public async Task<(bool success, GraphAuthResponse authResponse)> TryGetTokenAsync(GraphAccount account)
        {
            try
            {
                var result = await _JS.InvokeAsync<GetTokenResult>("getTokenAsync", _config);
                return (result?.IdToken != null, result.ToGraphAuthResponse());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, default);
            }
        }
    }
}
