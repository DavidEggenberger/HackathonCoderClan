using AutoMapper;
using DTOs.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

                var result = await userManager.CreateAsync(_user);

                gitHubAPIClient.SetAuthorizationHeader(info.Principal.Claims.Where(c => c.Type == "access_token").First().Value);
                GitHubRoot gitHubRoot = await gitHubAPIClient.GetGitHubRootAsync();
                GitHubRootEntity gitHubRootEntity = mapper.Map<GitHubRootEntity>(gitHubRoot);
                _user.GitHubRoot = gitHubRootEntity;
                await applicationDbContext.SaveChangesAsync();

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

            gitHubAPIClient.SetAuthorizationHeader(info.Principal.Claims.Where(c => c.Type == "access_token").First().Value);
            GitHubRoot gitHubRoot1 = await gitHubAPIClient.GetGitHubRootAsync();
            GitHubRootEntity gitHubRootEntit1y = mapper.Map<GitHubRootEntity>(gitHubRoot1);
            ApplicationUser appUser = applicationDbContext.Users.Include(s => s.GitHubRoot).Where(x => user.Id == x.Id).First();
            appUser.GitHubRoot = gitHubRootEntit1y;
            await applicationDbContext.SaveChangesAsync();

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            return signInResult switch
            {
                Microsoft.AspNetCore.Identity.SignInResult { Succeeded: true } => LocalRedirect("/"),
                _ => Redirect("/Error")
            };
        }
        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult> LogoutCurrentUser()
        {
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            appUser.TabsOpen = 0;
            appUser.IsOnline = false;
            await userManager.UpdateAsync(appUser);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await onlineHubContext.Clients.All.SendAsync("UpdateOnlineUsers");
            return Redirect("/");
        }
        [HttpGet("onlineUsers")]
        public ActionResult<List<OnlineUserDTO>> GetOnlineUsers()
        {
            return applicationDbContext.Users.Select(user => new OnlineUserDTO
            {
                Id = user.Id,
                IsOnline = user.IsOnline,
                PictureURI = user.PictureURI,
                UserName = user.UserName
            }).ToList();
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserDTO>> GetInfosOverMySelf()
        {
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return new CurrentUserDTO
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                PictureURI = appUser.PictureURI,
                PrivateProfile = appUser.PrivateProfile
            };
        }
        [HttpGet("BFFUser")]
        [AllowAnonymous]
        public ActionResult<BFFUserInfoDTO> GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BFFUserInfoDTO.Anonymous;
            }

            return new BFFUserInfoDTO()
            {
                Claims = User.Claims.Select(claim => new ClaimValueDTO { Type = claim.Type, Value = claim.Value }).ToList()
            };
        }
        [HttpGet("AllClanUsers")]
        [AllowAnonymous]
        public ActionResult<List<OnlineUserDTO>> GetAllClanUsers()
        {
            return applicationDbContext.Users.Include(user => user.GitHubRoot).Select(user => new OnlineUserDTO
            {
                bio = user.GitHubRoot.bio,
                blog = user.GitHubRoot.blog,
                html_url = user.GitHubRoot.html_url,
                location = user.GitHubRoot.location,
                Id = user.Id,
                twitter_username = user.GitHubRoot.twitter_username,
                login = user.GitHubRoot.login,
                UserName = user.UserName,
                PictureURI = user.PictureURI
            }).ToList();
        }
    }
}
