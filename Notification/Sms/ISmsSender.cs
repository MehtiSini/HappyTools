using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzPortal.Main.Notification;

namespace HappyTools.Notification.Sms
{
    public interface ISmsSender
    {
        Task<SendSmsResponse> SendByTemplateAsync(SendSmsByTemplateDto input);
    }

    public class SendSmsByTemplateDto
    {
        public string Template { get; set; }
        public string Recieptor { get; set; }
        public NotificationOtpTokenItem Tokens { get; set; }
    }

    public class SendSmsResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

}
