using Microsoft.AspNetCore.Mvc;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper.Models
{
    public class ApiProblemDetailsResponse : ProblemDetails
    {
        public bool IsError { get; set; }
    }
}
