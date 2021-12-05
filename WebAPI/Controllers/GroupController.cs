using DTOs.Group;
using DTOs.Messages;
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
using WebAPI.Hubs;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private IHubContext<OnlineHub> onlineHubContext;
        private ApplicationDbContext applicationDbContext;
        public GroupController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHubContext<OnlineHub> onlineHubContext, ApplicationDbContext applicationDbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.onlineHubContext = onlineHubContext;
            this.applicationDbContext = applicationDbContext;
        }
        [HttpPost("CreateGroup")]
        public async Task<ActionResult> CreateGroup(GroupDTO groupDTO)
        {
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            applicationDbContext.Groups.Add(new Group 
            { 
                Name = groupDTO.Name,
                ApplicationUsersInGroup = new List<ApplicationUserGroupMembership>
                {
                    new ApplicationUserGroupMembership
                    {
                        ApplicationUser = appUser
                    }
                }
            });
            await applicationDbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("GetAllGroups")]
        public async Task<List<GroupDTO>> GetAllGroups()
        {
            return applicationDbContext.Groups.Include(group => group.ApplicationUsersInGroup).Select(group => new GroupDTO
            {
                Name = group.Name,
                Id = group.Id,
                MembersId = group.ApplicationUsersInGroup.Select(s => s.ApplicationUserId).ToList(),
                CreatorId = group.CreatorApplicationUserId
            }).ToList();
        }
        [HttpGet("JoinGroup/{groupId}")]
        public async Task<ActionResult> JoinGroup(Guid groupId)
        {
            Group group = applicationDbContext.Groups.Include(s => s.ApplicationUsersInGroup).Where(group => group.Id == groupId).First();
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if(!group.ApplicationUsersInGroup.Select(s => s.ApplicationUserId).Contains(appUser.Id))
            {
                group.ApplicationUsersInGroup.Add(new ApplicationUserGroupMembership
                {
                    ApplicationUser = appUser
                });
                await applicationDbContext.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpGet("LeaveGroup/{groupId}")]
        public async Task<ActionResult> LeaveGroup(Guid groupId)
        {
            Group group = applicationDbContext.Groups.Include(s => s.ApplicationUsersInGroup).Where(group => group.Id == groupId).First();
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (group.ApplicationUsersInGroup.Select(s => s.ApplicationUserId).Contains(appUser.Id))
            {
                ApplicationUserGroupMembership applicationUserGroupMembership = group.ApplicationUsersInGroup.Where(s => s.ApplicationUserId == appUser.Id).First();
                group.ApplicationUsersInGroup.Remove(applicationUserGroupMembership);
                await applicationDbContext.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpGet("DeleteGroup/{groupId}")]
        public async Task<ActionResult> DeleteGroup(Guid groupId)
        {
            Group group = applicationDbContext.Groups.Include(s => s.ApplicationUsersInGroup).Where(group => group.Id == groupId).First();
            ApplicationUser appUser = await userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (group.CreatorApplicationUserId == null || group.CreatorApplicationUserId == appUser.Id)
            {
                applicationDbContext.Groups.Remove(group);
                await applicationDbContext.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpGet("MessagesForGroup/{groupId}")]
        public async Task<List<MessagesDTO>> GetMessagesPerGroup(Guid groupId)
        {
            Group group = applicationDbContext.Groups.Include(s => s.GroupMessages).ThenInclude(t => t.ApplicationUser).Where(group => group.Id == groupId).First();
            return group.GroupMessages.Select(message => new MessagesDTO
            {
                Content = message.Text,
                SenderUserName = message.ApplicationUser.UserName
            }).ToList();
        }
    }
}
