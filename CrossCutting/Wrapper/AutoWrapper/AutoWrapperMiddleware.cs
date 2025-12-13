using HappyTools.CrossCutting.Wrapper.AutoWrapper.Base;
using HappyTools.CrossCutting.Wrapper.AutoWrapper.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper
{
    internal class AutoWrapperMiddleware : WrapperBase
    {
        private readonly AutoWrapperMembers _awm;
        public AutoWrapperMiddleware(RequestDelegate next, IOptions<AutoWrapperOptions> options, ILogger<AutoWrapperMiddleware> logger, IActionResultExecutor<ObjectResult> executor) : base(next, options, logger, executor)
        {
            var jsonSettings = JSONHelper.GetJSONSettings(options.Value.IgnoreNullValue, options.Value.ReferenceLoopHandling, options.Value.UseCamelCaseNamingStrategy);
            _awm = new AutoWrapperMembers(options, logger, jsonSettings);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await InvokeAsyncBase(context, _awm);
        }
    }

    internal class AutoWrapperMiddleware<T> : WrapperBase
    {
        private readonly AutoWrapperMembers _awm;
        public AutoWrapperMiddleware(RequestDelegate next, IOptions<AutoWrapperOptions> options, ILogger<AutoWrapperMiddleware> logger, IActionResultExecutor<ObjectResult> executor) : base(next, options, logger, executor)
        {
            var (Settings, Mappings) = JSONHelper.GetJSONSettings<T>(options.Value.IgnoreNullValue, options.Value.ReferenceLoopHandling, options.Value.UseCamelCaseNamingStrategy);
            _awm = new AutoWrapperMembers(options, logger, Settings, Mappings, true);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await InvokeAsyncBase(context, _awm);
        }

    }
}
