using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HappyTools.Utilities.Extensions.HttpClient
{
    public interface IHttpService
    {
        string BaseAddress { get; set; }
        string Secret { get; }
        bool EnableCompression { get; set; }
        string Accept { get; set; }
        double TimeoutInSeconds { get; set; }
        event DataSendingCompletedEventHandler DataSendingCompleted;
        Task<HttpResponseMessage> PostChunkAsync<TObject>(TokenResponseModel token, string apiUrl, List<TObject> items, int chunkSize, bool continueOnChunkError);
        Task<HttpResponseMessage> PostChunkAsync(TokenResponseModel token, string apiUrl, List<object> items, int chunkSize, bool continueOnChunkError);
        Task<HttpResponseMessage> PostAsJsonAsync<TObject>(TokenResponseModel token, string apiUrl, TObject item);
        Task<HttpResponseMessage> PostAsJsonAsync<TObject>(TokenResponseModel token, string apiUrl, TObject item, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content);
        Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content, string accept);
        Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken);
        Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(TokenResponseModel token, string apiUrl, TSentObject item);
        Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(TokenResponseModel token, string apiUrl, TSentObject item, CancellationToken cancellationToken);
        Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content, string accept);
        Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken);
        Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content);
        Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content);
        Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content, string accept);
        Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsJsonAsync<TObject>(string apiUrl, TObject item);
        Task<HttpResponseMessage> PostAsJsonAsync<TObject>(string apiUrl, TObject item, CancellationToken cancellationToken);
        Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(string apiUrl, TSentObject item);
        Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(string apiUrl, TSentObject item, CancellationToken cancellationToken);
        Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content);
        Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content,
            CancellationToken cancellationToken);
        Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content, string accept);
        Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content, string accept,
            CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync(TokenResponseModel token, string apiUrl);
        Task<HttpResponseMessage> GetAsync(TokenResponseModel token, string apiUrl, CancellationToken cancellationToken);
        Task<TResultingObject> GetAsync<TResultingObject>(TokenResponseModel token, string apiUrl);
        Task<TResultingObject> GetAsync<TResultingObject>(TokenResponseModel token, string apiUrl, CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync(string apiUrl);
        Task<HttpResponseMessage> GetAsync(string apiUrl, CancellationToken cancellationToken);
        Task<TResultingObject> GetAsync<TResultingObject>(string apiUrl);
        Task<TResultingObject> GetAsync<TResultingObject>(string apiUrl, CancellationToken cancellationToken);
        Task<TokenResponseModel> GetBearerTokenAsync(string username, string password);
        System.Net.Http.HttpClient GetClient();
        System.Net.Http.HttpClient GetClient(string accept);
        System.Net.Http.HttpClient GetClient(TokenResponseModel token);
        System.Net.Http.HttpClient GetClient(bool enableCompression, string accept = "application/json");
        System.Net.Http.HttpClient GetClient(TokenResponseModel token, bool enableCompression, string accept = "application/json");
        Task<bool> CheckServerConnection();
    }
}