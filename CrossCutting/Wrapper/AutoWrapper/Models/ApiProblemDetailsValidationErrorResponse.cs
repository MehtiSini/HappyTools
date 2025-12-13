using HappyTools.CrossCutting.Wrapper.AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper.Models
{
    public class ApiProblemDetailsValidationErrorResponse : ProblemDetails
    {
        public bool IsError { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
    }
}
