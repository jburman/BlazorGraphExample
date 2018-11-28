using BlazorGraphExample.Services;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using W8lessLabs.GraphAPI;

namespace BlazorGraphExample
{
    
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var appState = new AppState();
            services.AddSingleton(appState);
            services.AddSingleton<IPagingState>(appState);

            services.AddSingleton(new AuthConfig(
                clientId: "CLIENT ID HERE",
                scopes: new[] { "https://graph.microsoft.com/user.read https://graph.microsoft.com/files.read" }
                ));

            services.AddSingleton<IJsonSerializer, JsonSerializer>(); // Used by HttpService
            services.AddSingleton<IHttpService, HttpService>();       // Used by GraphService
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IGraphService, GraphService>();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
