using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace YuzPortal.Microservice.Shared.Notification.KaveNeagr
{
    public class KaveNegarSmsService : IKaveNegarSmsService, IScopedDependency
    {
        private readonly IConfiguration _configuration;

        public KaveNegarSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<NotificationSentResult> SendSms(NotificationSendDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<NotificationSentResult> SendVerificationCode(SmsVerfiicationDto input)
        {
            try
            {
                var apiKey = _configuration["SmsApiKey"];

                if (apiKey.IsNullOrEmpty())
                    throw new Exception("Please Specify Sms ApiKey! ");

                var api = new Kavenegar.KavenegarApi(apiKey);

                var result = api.VerifyLookup(input.Receptor, input.Code, input.Template);

                return new NotificationSentResult();
            }
            catch (Kavenegar.Exceptions.ApiException ex)
            {
                return new NotificationSentResult(ex.Message);
            }
            catch (Kavenegar.Exceptions.HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                return new NotificationSentResult(ex.Message);
            }

        }

    }
}
