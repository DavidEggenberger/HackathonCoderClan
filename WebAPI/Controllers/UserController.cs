using AutoMapper;
using DTOs.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.GitHub;
using WebAPI.Hubs;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private IHubContext<OnlineHub> onlineHubContext;
        private ApplicationDbContext applicationDbContext;
        private GitHubAPIClient gitHubAPIClient;
        private IMapper mapper;
        public UserController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHubContext<OnlineHub> onlineHubContext, ApplicationDbContext applicationDbContext, GitHubAPIClient gitHubAPIClient, IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.onlineHubContext = onlineHubContext;
            this.applicationDbContext = applicationDbContext;
            this.gitHubAPIClient = gitHubAPIClient;
            this.mapper = mapper;
        }
        [HttpGet("Login")]
        public async Task<ActionResult> LoginRedirectToGitHub()
        {
            var redirectUrl = Url.Action("GitHubLoginCallback", "User");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
            return Challenge(properties, "GitHub");
        }
        [HttpGet]
        public async Task<IActionResult> GitHubLoginCallback(string ReturnUrl = null)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            var user = await userManager.FindByNameAsync(info.Principal.Identity.Name);

            if (info is not null && user is null)
            {
                ApplicationUser _user = new ApplicationUser
                {
                    UserName = info.Principal.Identity.Name,
                    IsOnline = false,
                    PictureURI = info.Principal.Claims.Where(claim => claim.Type == "picture").First().Value
                };

                gitHubAPIClient.SetAuthorizationHeader(info.Principal.Claims.Where(c => c.Type == "access_token").First().Value);
                GitHubRoot gitHubRoot = await gitHubAPIClient.GetGitHubRootAsync();
                GitHubRootEntity gitHubRootEntity = mapper.Map<GitHubRootEntity>(gitHubRoot);
                _user.GitHubRoot = gitHubRootEntity;

                var result = await userManager.CreateAsync(_user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(_user, info);
                    await signInManager.SignInAsync(_user, isPersistent: false, info.LoginProvider);
                    return LocalRedirect("/");
                }
            }

            string pictureURI = info.Principal.Claims.Where(claim => claim.Type == "picture").First().Value;
            if (user.PictureURI != pictureURI)
            {
                user.PictureURI = pictureURI;
                await userManager.UpdateAsync(user);
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            return signInResult switch
            {
                Microsoft.AspNetCore.Identity.SignInResult { Succeeded: true } => LocalRedirect("/"),
                _ => Redirect("/Error")
            };
        }
        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult<BFFUserInfoDTO>> LogoutCurrentUser()
        {
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            appUser.TabsOpen = 0;
            appUser.IsOnline = false;
            await userManager.UpdateAsync(appUser);
            await onlineHubContext.Clients.All.SendAsync("UpdateOnlineUsers");
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, IdentityConstants.ApplicationScheme, IdentityConstants.ExternalScheme);
        }
        [HttpGet("onlineUsers")]
        public ActionResult<List<UserDTO>> GetOnlineUsers()
        {
            return applicationDbContext.Users.Select(user => new UserDTO
            {
                
            }).ToList();
        }
    }
}
