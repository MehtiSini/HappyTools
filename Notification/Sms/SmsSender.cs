using HappyTools.DependencyInjection.Contracts;
using HappyTools.Notification.Sms.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification.Sms
{
    public class SmsSender : ISmsSender, IScopedDependency
    {
        private readonly ISmsProviderFactory _factory;

        public SmsSender(ISmsProviderFactory factory)
        {
            _factory = factory;
        }

        public async Task<SendSmsResponse> SendByTemplateAsync(SendSmsByTemplateDto input)
        {
            var provider = _factory.Create();

            return await provider.SendByPatternAsync(input);
        }
    }

}
