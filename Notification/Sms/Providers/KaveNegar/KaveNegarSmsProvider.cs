using HappyTools.DependencyInjection.Contracts;
using HappyTools.Notification.Sms;
using HappyTools.Notification.Sms.Providers;
using HappyTools.Utilities.Extensions;
using Kavenegar;
using Kavenegar.Exceptions;
using Kavenegar.Models;
using Kavenegar.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using YuzPortal.Main.Notification;

namespace HappyTools.Notification.Sms.Providers.KaveNegar
{
    public class KaveNegarSmsProvider : ISmsProvider, IScopedDependency
    {
        private readonly IConfiguration _configuration;

        public KaveNegarSmsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SendSmsResponse> SendByPatternAsync(SendSmsByTemplateDto input)
        {
            try
            {
                var apiKey = _configuration["SMS:ApiKey"];
                if (string.IsNullOrEmpty(apiKey)) throw new Exception("ApiKey Is Mandatory!");

                var api = new KavenegarApi(apiKey);

                var result = VerifyLookupWithArray(api, input.Recieptor, input.Template, input.Tokens);

                return new SendSmsResponse { Message = result.Message, Success = true };
            }
            catch (KavenegarException ex)
            {
                return new SendSmsResponse { Message = ex.Message, Success = false };
            }
        }



        public SendResult VerifyLookupWithArray(
      KavenegarApi api,
      string receptor,
      string template,
      NotificationOtpTokenItem tokenObj,
      VerifyLookupType type = VerifyLookupType.Sms)
        {
            try
            {
                if (tokenObj == null)
                    return new SendResult { Status = 400, Message = "Tokens is null." };


                return api.VerifyLookup(receptor, tokenObj.Token, tokenObj.Token2, tokenObj.Token3, tokenObj.Token10, tokenObj.Token20, template, type);


                //// فقط توکن‌های جایگاهی VerifyLookup
                //var t1 = tokenObj.Token;
                //var t2 = tokenObj.Token2;
                //var t3 = tokenObj.Token3;

                //// اگر توکن 2 یا 3 پر شده ولی توکن قبلی خالی است => ترتیب اشتباه است
                //if (string.IsNullOrWhiteSpace(t1) && (!string.IsNullOrWhiteSpace(t2) || !string.IsNullOrWhiteSpace(t3)))
                //    return new SendResult { Status = 400, Message = "Token is required when Token2/Token3 is set." };

                //if (string.IsNullOrWhiteSpace(t2) && !string.IsNullOrWhiteSpace(t3))
                //    return new SendResult { Status = 400, Message = "Token2 is required when Token3 is set." };

                //// توجه: Token10/Token20 را اینجا دخالت نمی‌دهیم
                //// چون باعث می‌شود overload اشتباه انتخاب شود.
                //// اگر Template شما واقعاً Token10/Token20 می‌خواهد، باید overload مناسب کاوه‌نگار را جداگانه صدا بزنی
                //// یا قبل از این متد تبدیلش کنی به Token2/Token3.

                //if (!string.IsNullOrWhiteSpace(t1) && string.IsNullOrWhiteSpace(t2) && string.IsNullOrWhiteSpace(t3))
                //{
                //    return api.VerifyLookup(receptor, t1, template, type);
                //}

                //if (!string.IsNullOrWhiteSpace(t1) && !string.IsNullOrWhiteSpace(t2) && string.IsNullOrWhiteSpace(t3))
                //{
                //    // بعضی نسخه‌ها پارامتر سوم (token3) را می‌خواهند، اگر ندارید null بفرست
                //    return api.VerifyLookup(receptor, t1, t2, null, template, type);
                //}

                //if (!string.IsNullOrWhiteSpace(t1) && !string.IsNullOrWhiteSpace(t2) && !string.IsNullOrWhiteSpace(t3))
                //{
                //    return api.VerifyLookup(receptor, t1, t2, t3, template, type);
                //}

                //// اگر همه خالی باشند
                //return new SendResult { Status = 400, Message = "At least Token must be set." };
            }
            catch (Exception ex)
            {
                return new SendResult { Status = 500, Message = ex.Message };
            }
        }



    }
}
