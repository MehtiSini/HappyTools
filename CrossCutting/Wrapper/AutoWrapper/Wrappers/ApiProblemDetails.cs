using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using static HappyTools.CrossCutting.Wrapper.AutoWrapper.Wrappers.ApiProblemDetailsMember;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper.Wrappers
{

    internal class ApiProblemDetails : ProblemDetails
    {
        public ApiProblemDetails(int statusCode)
        {
            IsError = true;
            Status = statusCode;
            Type = $"https://httpstatuses.com/{statusCode}";
            Title = ReasonPhrases.GetReasonPhrase(statusCode);
        }

        public ApiProblemDetails(ProblemDetails details)
        {
            IsError = true;
            Details = details;
        }

        [JsonProperty(Order = -2)]
        public bool IsError { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        public ErrorDetails Errors { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
        [JsonIgnore]
        public ProblemDetails Details { get; set; }
    }
}
