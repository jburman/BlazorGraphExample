using BlazorGraphExample.Services;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorGraphExample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddSingleton(new AppState());

                services.AddSingleton(new AuthConfig(
                    clientId: "CLIENT ID HERE",
                    scopes: new[] { "https://graph.microsoft.com/user.read https://graph.microsoft.com/files.read" }
                    ));

                services.AddSingleton<IAuthService, AuthService>();
                services.AddSingleton<IGraphService, GraphService>();
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
