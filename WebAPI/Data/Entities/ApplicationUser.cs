using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool PrivateProfile { get; set; }
        public int TabsOpen { get; set; }
        public bool IsOnline { get; set; }
        public string PictureURI { get; set; }
        public GitHubRootEntity GitHubRoot { get; set; }
        public List<Learnings> UpcomingLearnings { get; set; }
        public List<ApplicationUserGroupMembership> JoinedGroups { get; set; }
        public List<Message> SentMessages { get; set; }
    }
}
