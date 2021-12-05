using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Group
{
    public class GroupDTO
    {
        public Guid Id { get; set; }
        public List<string> MembersId { get; set; }
        public string Name { get; set; }
    }
}
