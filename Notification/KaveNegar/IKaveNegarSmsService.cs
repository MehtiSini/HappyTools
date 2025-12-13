using HappyTools.Notification;

namespace HappyTools.Notification.KaveNegar
{

    public interface IKaveNegarSmsService
    {
        public Task<NotificationSentResult> SendVerificationCode(SmsVerfiicationDto input);
        public Task<NotificationSentResult> SendSms(NotificationSendDto input);
        public Task<List<NotificationSentResult>> SendBulkSms(List<NotificationSendDto> notificaitons);
    }
}
