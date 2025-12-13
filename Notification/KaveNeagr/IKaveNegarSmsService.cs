namespace YuzPortal.Microservice.Shared.Notification.KaveNeagr
{

    public interface IKaveNegarSmsService
    {
        public Task<NotificationSentResult> SendVerificationCode(SmsVerfiicationDto input);
        public Task<NotificationSentResult> SendSms(NotificationSendDto input);
    }
}
