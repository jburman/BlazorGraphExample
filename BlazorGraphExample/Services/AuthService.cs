using Microsoft.AspNetCore.Blazor.Browser.Interop;
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

        public bool IsLoggedIn() =>
            RegisteredFunction.Invoke<bool>("isLoggedIn", _config);

        public string GetUserName() =>
            RegisteredFunction.Invoke<string>("getUserName", _config);

        public void Login() =>
            RegisteredFunction.Invoke<string>("loginRedirect", _config);

        public void Logout() =>
            RegisteredFunction.Invoke<string>("logout", _config);

        public async Task<bool> TryCompleteLoginAsync() =>
            await RegisteredFunction.InvokeAsync<bool>("completeLoginAsync", _config);

        public async Task<(bool success, string idToken)> TryGetTokenAsync()
        {
            var result = await RegisteredFunction.InvokeAsync<GetTokenResult>("getTokenAsync", _config);

            if (result != null)
                return (true, result.IdToken);
            else
                return (false, null);
        }
    }
}
