using HappyTools.DependencyInjection.Contracts;
using HappyTools.Notification.KaveNegar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification
{
    public class NotificationSenderAppService : INotificationSenderAppService, ITransientDependency
    {
        // might be handled better
        private readonly IKaveNegarSmsService _kaveNegarSmsService;

        public NotificationSenderAppService(IKaveNegarSmsService kaveNegarSmsService)
        {
            _kaveNegarSmsService = kaveNegarSmsService;
        }

        public async Task SendNotifications(List<NotificationSendDto> notifications)
        {
           await _kaveNegarSmsService.SendBulkSms(notifications);
        }
    }
}
