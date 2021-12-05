using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using IServiceScope serviceScope = host.Services.CreateScope();
            ApplicationDbContext appDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if(appDbContext.Users.Count() < 2)
            {
                appDbContext.Users.Add(new ApplicationUser
                {
                    UserName = "HappyProgrammer",
                    GitHubRoot = new GitHubRootEntity
                    {
                        bio = "Learning how to code",
                        blog = "https://jimmybogard.com/",
                        html_url = "https://www.github.com",
                        location = "The World",
                        twitter_username = "TwitterUserName",
                        login = "HappyProgrammer",
                        repos_url = "https://www.github.com"
                    },
                    PictureURI = "https://www.history.com/.image/ar_16:9%2Cc_fill%2Ccs_srgb%2Cfl_progressive%2Cq_auto:good%2Cw_1200/MTU3ODc4NjAwMjkxNzIyNTY5/yosemite-3.jpg"
                });
                await appDbContext.SaveChangesAsync();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingcontext, config) =>
                {
                    config.AddAzureKeyVault(new Uri("https://coderclankeyvault.vault.azure.net/"),
                        new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = "3def3d2b-3bc3-4ce4-984f-1a29fe437722" }));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
