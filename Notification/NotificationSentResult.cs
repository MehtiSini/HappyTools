using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification
{
    public class NotificationSentResult
    {
        public bool IsFailed { get; set; }
        public string Message { get; set; }

        //When it's Ok
        public NotificationSentResult()
        {

        }

        //When it Fails
        public NotificationSentResult(string message)
        {
            IsFailed = true;
            Message = message;
        }
    }
}
