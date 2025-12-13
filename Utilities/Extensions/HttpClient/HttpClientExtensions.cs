using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using HappyTools.Utilities.Extensions.HttpClient.HttpError;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HappyTools.Utilities.Extensions.HttpClient
{
    public static class HttpClientExtensions
    {

        public static Task<string> GetStringAsync(this System.Net.Http.HttpClient httpClient, Uri requestUri, CancellationToken cancellationToken)
        {

            return httpClient.GetContentAsync(
                requestUri, HttpCompletionOption.ResponseContentRead, string.Empty,
                (content) => content.ReadAsStringAsync(), cancellationToken);
        }

        public static Task<string> GetStringAsync(this System.Net.Http.HttpClient httpClient, string requestUri, CancellationToken cancellationToken)
        {

            return httpClient.GetContentAsync(
                requestUri, HttpCompletionOption.ResponseContentRead, string.Empty,
                (content) => content.ReadAsStringAsync(), cancellationToken);
        }

        public static Task<T> GetContentAsync<T>(this System.Net.Http.HttpClient httpClient, string requestUri, HttpCompletionOption completionOption, T defaultValue, Func<HttpContent, Task<T>> readAs)
        {

            return httpClient.GetContentAsync(requestUri, completionOption, defaultValue, readAs, CancellationToken.None);
        }

        public static Task<T> GetContentAsync<T>(this System.Net.Http.HttpClient httpClient, string requestUri, HttpCompletionOption completionOption, T defaultValue, Func<HttpContent, Task<T>> readAs, CancellationToken cancellationToken)
        {

            return httpClient.GetContentAsync(createUri(requestUri), completionOption, defaultValue, readAs, cancellationToken);
        }

        public static Task<T> GetContentAsync<T>(this System.Net.Http.HttpClient httpClient, Uri requestUri, HttpCompletionOption completionOption, T defaultValue, Func<HttpContent, Task<T>> readAs)
        {

            return httpClient.GetContentAsync(requestUri, completionOption, defaultValue, readAs, CancellationToken.None);
        }

        public static Task<T> GetContentAsync<T>(this System.Net.Http.HttpClient httpClient, Uri requestUri, HttpCompletionOption completionOption, T defaultValue, Func<HttpContent, Task<T>> readAs, CancellationToken cancellationToken)
        {

            var tcs = new TaskCompletionSource<T>();
            httpClient.GetAsync(requestUri, completionOption, cancellationToken).ContinueWithStandard(requestTask =>
            {

                if (HandleRequestFaultsAndCancelation(requestTask, tcs))
                {
                    return;
                }

                var result = requestTask.Result;

                if (result.Content == null)
                {

                    tcs.TrySetResult(defaultValue);
                    return;
                }

                try
                {

                    readAs(result.Content).ContinueWithStandard(contentTask =>
                    {

                        if (!HandleFaultsAndCancelation(contentTask, tcs))
                        {
                            tcs.TrySetResult(contentTask.Result);
                        }
                    });
                }
                catch (Exception exception)
                {

                    tcs.TrySetException(exception);
                }
            });

            return tcs.Task;
        }

        // System.Net.Http.HttpClient
        private static Uri createUri(string uri)
        {

            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        // System.Net.Http.HttpClient
        private static bool HandleRequestFaultsAndCancelation<T>(Task<HttpResponseMessage> task, TaskCompletionSource<T> tcs)
        {

            if (HandleFaultsAndCancelation(task, tcs))
            {
                return true;
            }

            var result = task.Result;

            if (!result.IsSuccessStatusCode)
            {

                if (result.Content != null)
                {
                    result.Content.Dispose();
                }

                tcs.TrySetException(
                    new HttpRequestException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Request is not a success",
                            new object[] {
                            (int)result.StatusCode, result.ReasonPhrase })));
                return true;
            }

            return false;
        }

        // System.Net.Http.HttpUtilities
        private static bool HandleFaultsAndCancelation<T>(Task task, TaskCompletionSource<T> tcs)
        {

            if (task.IsFaulted)
            {
                tcs.TrySetException(task.Exception.GetBaseException());
                return true;
            }

            if (task.IsCanceled)
            {
                tcs.TrySetCanceled();
                return true;
            }

            return false;
        }
        private static readonly Encoding requestEncoding = new UTF8Encoding(false);



        public static Task<KeyValuePair<HttpResponseMessage, TResult>> GetAsync<TResult>(this System.Net.Http.HttpClient source)
        {
            return source.GetAsync<TResult>((Uri)null, CancellationToken.None);
        }

        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> GetAsync<TResult>(this System.Net.Http.HttpClient source, string url)
        {
            return await source.GetAsync<TResult>(url, CancellationToken.None);
        }
        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> GetAsync<TResult>(this System.Net.Http.HttpClient source, string url, CancellationToken token)
        {
            var response = await source.GetAsync(url, token);
            var resStr = await response.Content.ReadAsStringAsync();
            var res = await DecodeResponseAsync<TResult>(response);

            return new KeyValuePair<HttpResponseMessage, TResult>(response, res);
        }



        public static Task<KeyValuePair<HttpResponseMessage, TResult>> GetAsync<TResult>(this System.Net.Http.HttpClient source, Uri url)
        {
            return source.GetAsync<TResult>(url, CancellationToken.None);
        }


        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> GetAsync<TResult>(this System.Net.Http.HttpClient source, Uri url, CancellationToken token)
        {
            var response = await source.GetAsync(url, token);
            var res = await DecodeResponseAsync<TResult>(response);
            return new KeyValuePair<HttpResponseMessage, TResult>(response, res);
        }



        public static Task<KeyValuePair<HttpResponseMessage, TResult>> PostAsync<TResult>(this System.Net.Http.HttpClient source, HttpContent content)
        {
            return source.PostAsync<TResult>(null, content, CancellationToken.None);
        }



        public static Task<KeyValuePair<HttpResponseMessage, TResult>> PostAsync<TResult>(this System.Net.Http.HttpClient source, HttpContent content, CancellationToken token)
        {
            return source.PostAsync<TResult>(null, content, token);
        }



        public static Task<KeyValuePair<HttpResponseMessage, TResult>> PostAsync<TResult>(this System.Net.Http.HttpClient source, string url, HttpContent content)
        {
            return source.PostAsync<TResult>(url, content, CancellationToken.None);
        }


        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> PostAsync<TResult>(this System.Net.Http.HttpClient source, string url, HttpContent content, CancellationToken token)
        {
            var response = await source.PostAsync(url, content, token);
            var res = await DecodeResponseAsync<TResult>(response);
            return new KeyValuePair<HttpResponseMessage, TResult>(response, res);
        }
        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> PutAsync<TResult>(this System.Net.Http.HttpClient source, string url, HttpContent content, CancellationToken token)
        {
            var response = await source.PutAsync(url, content, token);
            var res = await DecodeResponseAsync<TResult>(response);
            return new KeyValuePair<HttpResponseMessage, TResult>(response, res);
        }
        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> DeleteAsync<TResult>(this System.Net.Http.HttpClient source, string url, CancellationToken token)
        {
            var response = await source.DeleteAsync(url, token);
            var res = await DecodeResponseAsync<TResult>(response);
            return new KeyValuePair<HttpResponseMessage, TResult>(response, res);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this System.Net.Http.HttpClient source, string url, TValue value)
        {
            return await source.PostAsJsonAsync(url, value, CancellationToken.None);
        }


        public static async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this System.Net.Http.HttpClient source, string url, TValue value, CancellationToken token)
        {
            var httpResponseMessage = await source.PostAsync(url, EncodeRequest(value), token);
            return httpResponseMessage;
        }


        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> PostAsJsonAsync<TValue, TResult>(this System.Net.Http.HttpClient source, string url, TValue value)
        {
            return await source.PostAsJsonAsync<TValue, TResult>(url, value, CancellationToken.None);
        }


        public static async Task<KeyValuePair<HttpResponseMessage, TResult>> PostAsJsonAsync<TValue, TResult>(this System.Net.Http.HttpClient source, string url, TValue value, CancellationToken token)
        {
            return await source.PostAsync<TResult>(url, EncodeRequest(value), token);
        }



        internal static HttpContent EncodeRequest<TValue>(TValue value)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, requestEncoding))
                {
                    var jsonSerializer = new JsonSerializer();
                    using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonTextWriter.Formatting = jsonSerializer.Formatting;
                        jsonSerializer.Serialize(jsonTextWriter, value);
                        jsonTextWriter.Flush();
                        var byteArrayContent = new ByteArrayContent(memoryStream.ToArray());
                        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json")
                        {
                            CharSet = requestEncoding.WebName
                        };
                        return byteArrayContent;
                    }
                }
            }
        }



        internal static async Task<TResult> DecodeResponseAsync<TResult>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var resStr = await response.Content.ReadAsStringAsync();
                if (typeof(TResult) == typeof(string))
                    return (TResult)Convert.ChangeType(resStr, typeof(TResult));
                return JsonConvert.DeserializeObject<TResult>(resStr);
            }
            //string format = "Response status code does not indicate success: {0} ({1}).";
            //object[] objArray = new object[2]
            //{
            //    (object) response.StatusCode,
            //    (object) response.ReasonPhrase
            //};
            var exception = response.CreateApiException();
            //throw new ApiException(response, string.Format(format, objArray));
            throw exception;
        }
        public static ApiException CreateApiException(this HttpResponseMessage response)
        {
            var httpErrorStr = response.Content.ReadAsStringAsync().Result;
            HappyTools.Utilities.Extensions.HttpClient.HttpError.HttpError httpErrorObject = null;
            //first try to get http error object
            try
            {
                httpErrorObject =
                    JsonConvert.DeserializeObject<HappyTools.Utilities.Extensions.HttpClient.HttpError.HttpError>(httpErrorStr);
                //MessageBox.Show(httpErrorObject.Message);


            }
            catch
            {
                httpErrorObject = null;
                // ignored
            }
            if (httpErrorObject != null)
            {
                if (httpErrorObject.ContainsKey("error_description"))
                    httpErrorObject.Message = httpErrorObject["error_description"].ToString();
                if (httpErrorObject.ContainsKey("errors"))
                    httpErrorObject.Message = httpErrorObject["errors"].ToString();
                var httpErrorException = new ApiException(response, httpErrorObject.Message);
                throw httpErrorException;
            }
            // Create an anonymous object to use as the template for deserialization:
            var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };
            var anonymousGrantErrorObject = new { error = "", error_description = "" };

            // Deserialize:
            try
            {
                var deserializedErrorObject =
                    JsonConvert.DeserializeAnonymousType(httpErrorStr, anonymousErrorObject);

                // Now wrap into an exception which best fullfills the needs of your application:
                var ex = new ApiException(response, deserializedErrorObject.message);

                // Sometimes, there may be Model Errors:
                if (deserializedErrorObject.ModelState != null)
                {
                    var errors =
                        deserializedErrorObject.ModelState
                            .Select(kvp => string.Join(". ", kvp.Value));
                    for (var i = 0; i < errors.Count(); i++)
                    {
                        // Wrap the errors up into the base Exception.Data Dictionary:
                        ex.Data.Add(i, errors.ElementAt(i));
                    }
                }
                // Othertimes, there may not be Model Errors:
                else
                {
                    var error =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorStr);
                    foreach (var kvp in error)
                    {
                        // Wrap the errors up into the base Exception.Data Dictionary:
                        ex.Data.Add(kvp.Key, kvp.Value);
                    }
                }
                return ex;
            }
            catch (Exception e)
            {
            }
            try
            {
                var deserializedGrantErrorObject =
                    JsonConvert.DeserializeAnonymousType(httpErrorStr, anonymousGrantErrorObject);
                var ex2 = new ApiException(response, deserializedGrantErrorObject.error_description);
                return ex2;
            }
            catch (Exception e)
            {
            }
            var ex3 = new ApiException(response);
            return ex3;
        }
    }


}
