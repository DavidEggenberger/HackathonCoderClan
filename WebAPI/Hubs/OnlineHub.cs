using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI.Hubs
{
    public class OnlineHub : Hub
    {
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext applicationDbContext;
        public OnlineHub(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }
        public override async Task OnConnectedAsync()
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                return;
            }
            ApplicationUser appUser = await userManager.FindByIdAsync(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (appUser.IsOnline is false)
            {
                appUser.IsOnline = true;
                appUser.TabsOpen = 1;
                await userManager.UpdateAsync(appUser);
                await Clients.All.SendAsync("UpdateOnlineUsers");
                return;
            }
            if (appUser.IsOnline)
            {
                appUser.TabsOpen++;
                await userManager.UpdateAsync(appUser);
            }
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                return;
            }
            ApplicationUser appUser = await userManager.FindByIdAsync(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (appUser.TabsOpen > 0)
            {
                appUser.TabsOpen--;
                await userManager.UpdateAsync(appUser);
            }
            if (appUser.TabsOpen == 0)
            {
                appUser.IsOnline = false;
                await userManager.UpdateAsync(appUser);
                await Clients.AllExcept(appUser.Id).SendAsync("UpdateOnlineUsers");
            }
        }
    }
}
