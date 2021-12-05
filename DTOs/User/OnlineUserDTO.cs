using DTOs.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.User
{
    public class OnlineUserDTO
    {
        public string Id { get; set; }
        public string PictureURI { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; }
        public string location { get; set; }
        public string bio { get; set; }
        public string twitter_username { get; set; }
        public string login { get; set; }
        public string html_url { get; set; }
        public string blog { get; set; }
        public string repoURI { get; set; }
        public List<GroupDTO> Groups { get; set; }
    }
}
