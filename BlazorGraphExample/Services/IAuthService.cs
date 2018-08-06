using System.Threading.Tasks;

namespace BlazorGraphExample.Services
{
    public interface IAuthService
    {
        string GetUserName();
        bool IsLoggedIn();
        void Login();
        void Logout();
        Task<(bool success, string idToken)> TryGetTokenAsync();
    }
}