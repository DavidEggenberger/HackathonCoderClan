﻿using System;
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
    }
}