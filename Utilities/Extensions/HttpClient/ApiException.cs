using System;
using System.Net;
using System.Net.Http;

namespace HappyTools.Utilities.Extensions.HttpClient
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }

        public ApiException(HttpResponseMessage response, string message) : base(message)
        {
            Response = response;
        }
        public ApiException(HttpResponseMessage response)
        {
            Response = response;
        }

        public HttpStatusCode StatusCode {
            get {
                return Response.StatusCode;
            }
        }

        //public IEnumerable<string> Errors
        //{
        //    get
        //    {
        //        return this.Data.Values.Cast<string>().ToList();
        //    }
        //}
    }
}