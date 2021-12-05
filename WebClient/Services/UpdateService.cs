using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class UpdateService
    {
        public event Func<Task> UpdateEvent;
        public void Invoke()
        {
            UpdateEvent?.Invoke();
        }
    }
}
