using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int TabsOpen { get; set; }
        public bool IsOnline { get; set; }
        public string PictureURI { get; set; }
        public GitHubRootEntity GitHubRoot { get; set; }
    }
}
