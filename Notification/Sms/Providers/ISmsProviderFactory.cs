using HappyTools.DependencyInjection.Contracts;
using HappyTools.Notification.Sms.Providers.KaveNegar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Notification.Sms.Providers
{
    public interface ISmsProviderFactory
    {
        ISmsProvider Create();
    }

    public class SmsProviderFactory : ISmsProviderFactory, IScopedDependency
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public SmsProviderFactory(
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public ISmsProvider Create()
        {
            var provider = _configuration["SMS:Provider"];

            return provider switch
            {
                "KaveNegar" => _serviceProvider.GetRequiredService<ISmsProvider>(),
                _ => throw new Exception($"SMS provider '{provider}' not supported")
            };
        }

    }

}
