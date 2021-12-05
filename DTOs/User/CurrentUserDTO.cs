using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.User
{
    public class CurrentUserDTO
    {
        public string Id { get; set; }
        public string PictureURI { get; set; }
        public bool PrivateProfile { get; set; }
        public string UserName { get; set; }
    }
}
