using HappyTools.DependencyInjection.Contracts;
using HappyTools.Utilities.Extensions;
using Kavenegar.Models;
using Microsoft.Extensions.Configuration;

namespace HappyTools.Notification.KaveNegar
{
    public class KaveNegarSmsService : IKaveNegarSmsService, IScopedDependency
    {
        private readonly IConfiguration _configuration;

        public KaveNegarSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<List<NotificationSentResult>> SendBulkSms(List<NotificationSendDto> notifications)
        {
            var results = new List<NotificationSentResult>();

            var apiKey = _configuration["SmsApiKey"];
            var sender = _configuration["Sender"];

            foreach (var notification in notifications)
            {
                var sendResult = new SendResult();

                var resultMessage = "";

                try
                {
                    if (apiKey.IsNullOrEmpty())
                        throw new Exception("Please Specify The ApiKey! ");

                    var api = new Kavenegar.KavenegarApi(apiKey);

                    sendResult = api.Send(sender, notification.ReceptorMobile, notification.Body);


                }
                catch (Exception ex)
                {
                    resultMessage = ex.Message;
                }

                var isFailed = false;

                if (sendResult.IsNotNull())
                {
                    resultMessage = sendResult.Message;
                }
                else
                {
                    isFailed = true;
                }

                results.Add(new NotificationSentResult
                {
                    Message = resultMessage,
                    IsFailed = isFailed
                });


            }
            return results;

        }

        public async Task<NotificationSentResult> SendSms(NotificationSendDto input)
        {
            try
            {
                var apiKey = _configuration["SmsApiKey"];
                var sender = _configuration["Sender"];

                if (apiKey.IsNullOrEmpty())
                    throw new Exception("Please Specify The ApiKey! ");

                if (sender.IsNullOrEmpty())
                    throw new Exception("Please Specify The Sender! ");

                var api = new Kavenegar.KavenegarApi(apiKey);

                var result = api.Send(sender, input.ReceptorMobile, input.Body);

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
