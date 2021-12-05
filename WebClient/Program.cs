using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebClient.Authentication;

namespace WebClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");


            builder.Services.AddAuthorizationCore(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
            builder.Services.AddTransient<AuthorizedHandler>();
            builder.Services.AddScoped<AuthenticationStateProvider, HostAuthenticationStateProvider>();
            builder.Services.AddHttpClient("unauthorizedClient", client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            });
            builder.Services.AddHttpClient("authorizedClient", client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<AuthorizedHandler>();
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("unauthorizedClient"));

            await builder.Build().RunAsync();
        }
    }
}
