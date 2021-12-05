using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Messages
{
    public class MessagesDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string SenderUserName { get; set; }
        public Guid GroupId { get; set; }
        public DateTime SentTime { get; set; }
    }
}
