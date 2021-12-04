using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        public UserController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        [HttpGet("Login")]
        public async Task<ActionResult> LoginRedirectToGitHub()
        {
            var redirectUrl = Url.Action("GitHubLoginCallback", "User");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
            return Challenge(properties, "GitHub");
        }
        public async Task<IActionResult> ExternalLoginCallback(string ReturnUrl = null)
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
            }
            await userManager.UpdateAsync(user);
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            return signInResult switch
            {
                Microsoft.AspNetCore.Identity.SignInResult { Succeeded: true } => LocalRedirect("/"),
                _ => Redirect("/Error")
            };
        }
    }
}
