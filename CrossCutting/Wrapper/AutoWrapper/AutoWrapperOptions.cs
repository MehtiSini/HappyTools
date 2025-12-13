using HappyTools.CrossCutting.Wrapper.AutoWrapper.Base;
using Newtonsoft.Json;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper
{
    public class AutoWrapperOptions : OptionBase
    {
        public bool UseCustomSchema { get; set; } = false;
        public ReferenceLoopHandling ReferenceLoopHandling { get; set; } = ReferenceLoopHandling.Ignore;
        public bool UseCustomExceptionFormat { get; set; } = false;
        public bool UseApiProblemDetailsException { get; set; } = false;
        public bool LogRequestDataOnException { get; set; } = true;
        public bool IgnoreWrapForOkRequests { get; set; } = false;
        public bool IgnoreStartupPath { get; set; } = true;
        public bool ShouldLogRequestData { get; set; } = true;

        public string SwaggerPath { get; set; } = "/swagger";
        public string OpenIDWellKnownPath { get; set; } = "/.well-known";
        public string OpenIDPath { get; set; } = "/connect";
        public IEnumerable<AutoWrapperExcludePath> ExcludePaths { get; set; } = null;
    }
}
