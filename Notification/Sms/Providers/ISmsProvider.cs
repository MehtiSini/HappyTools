using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification.Sms.Providers
{
    public interface ISmsProvider
    {
        Task<SendSmsResponse> SendByPatternAsync(SendSmsByTemplateDto input);
    }

}
