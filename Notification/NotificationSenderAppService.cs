using HappyTools.DependencyInjection.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification
{
    public class NotificationSenderAppService : INotificationSenderAppService, ITransientDependency
    {
        public Task SendNotifications(List<NotificationSendDto> notification)
        {
            throw new NotImplementedException();
        }
    }
}
