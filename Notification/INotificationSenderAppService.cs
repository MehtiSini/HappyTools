using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyTools.Notification
{
    public interface INotificationSenderAppService
    {
        public Task SendNotifications(List<NotificationSendDto> notification);
    }
}
