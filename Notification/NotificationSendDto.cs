using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification
{
    public class NotificationSendDto
    {
        public string ReceptorMobile { get; set; }
        public string ReceptorEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
