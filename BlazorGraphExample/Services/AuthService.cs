using Microsoft.JSInterop;
using System.Threading.Tasks;

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

        public bool IsLoggedIn() =>
            _JS.Invoke<bool>("isLoggedIn", _config);

        public string GetUserName() =>
            _JS.Invoke<string>("getUserName", _config);

        public void Login() =>
            _JS.Invoke<string>("loginRedirect", _config);

        public void Logout() =>
            _JS.Invoke<string>("logout", _config);

        public async Task<(bool success, string idToken)> TryGetTokenAsync()
        {
            var result = await _JS.InvokeAsync<GetTokenResult>("getTokenAsync", _config);

            if (result != null)
                return (true, result.IdToken);
            else
                return (false, null);
        }
    }
}
