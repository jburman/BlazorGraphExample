using BlazorGraphExample.Services;
using Microsoft.AspNetCore.Components.Builder;
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

            var authConfig = new AuthConfig(
                clientId: "CLIENT ID HERE",
                scopes: new[] { "https://graph.microsoft.com/user.read https://graph.microsoft.com/files.read" }
                );
            var authService = new AuthService(authConfig);

            services.AddSingleton<IJsonSerializer, JsonSerializer>();   // Used by HttpService
            services.AddSingleton<IHttpService, HttpService>();         // Used by GraphService
            services.AddSingleton(authService);
            services.AddSingleton<IAuthTokenProvider>(authService);     // Used by GraphService
            services.AddSingleton<IGraphService, GraphService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
