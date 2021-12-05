using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Data.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public string Text { get; set; }
    }
}
