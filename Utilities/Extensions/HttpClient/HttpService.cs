using System.Net.Http.Headers;
using System.Text;
using HappyTools.Utilities.Extensions.HttpClient.CompressionHandler;
using HappyTools.Utilities.Extensions.HttpClient.CompressionHandler.Compressors;
using Newtonsoft.Json;

namespace HappyTools.Utilities.Extensions.HttpClient
{
    // A delegate type for hooking up change notifications.
    public delegate void DataSendingCompletedEventHandler(object sender, DataSendingCompletedEventArgs e);

    public class HttpService : IHttpService
    {
        private static TokenResponseModel _token;
        private static HttpService instance;
        private double timeoutInSeconds = 100;
        private string baseAddress;
        private string _secret;
        private bool _enableCompression = true;
        private string _accept = "application/json";


        public string BaseAddress {
            get {
                return baseAddress;
            }
            set {
                baseAddress = value;
            }
        }

        public static TokenResponseModel Token {
            get {
                return _token;
            }
            set { _token = value; }
        }
        public string Secret {
            get {
                return _secret;
            }
        }

        public bool EnableCompression {
            get { return _enableCompression; }
            set { _enableCompression = value; }
        }

        public string Accept {
            get { return _accept; }
            set { _accept = value; }
        }

        public double TimeoutInSeconds {
            get {
                return timeoutInSeconds;
            }
            set {
                timeoutInSeconds = value;
            }
        }
        public HttpService(string baseAddress) : this(baseAddress, true)
        {
        }

        public HttpService(string baseAddress, bool enableCompression) : this(baseAddress, "application/json", enableCompression)
        {
        }
        public HttpService(string baseAddress, string accept, bool enableCompression)
        {
            baseAddress = baseAddress.Trim();
            //if (baseAddress.EndsWith("/"))
            //    baseAddress = baseAddress.Substring(0, baseAddress.Length - 1);
            BaseAddress = baseAddress;
            Accept = accept;
            EnableCompression = enableCompression;
        }
        //public static HttpService GetSingletonInstance(string baseAddress)
        //{
        //    return GetSingletonInstance(baseAddress, "application/json", true);
        //}
        public static HttpService GetSingletonInstance(string baseAddress, string accept = "application/json", bool enableCompression = true)
        {
            if (instance == null)
                instance = new HttpService(baseAddress, accept, enableCompression);
            return instance;
        }
        public event DataSendingCompletedEventHandler DataSendingCompleted;

        // Invoke the DataChunkCompleted event; called whenever a data chunk sent
        protected virtual void OnDataSendingCompleted(DataSendingCompletedEventArgs e)
        {
            if (DataSendingCompleted != null)
                DataSendingCompleted(this, e);
        }
        public async Task<HttpResponseMessage> PostChunkAsync<TObject>(TokenResponseModel token, string apiUrl, List<TObject> items, int chunkSize, bool continueOnChunkError)
        {
            if (items == null)
                throw new ArgumentException("items cannot be null!");
            var allItemsCount = items.Count;
            var startIndex = 0;
            HttpResponseMessage res = null;
            if (startIndex == 0 && allItemsCount < chunkSize)
            {
                res = await PostAsJsonAsync(token, apiUrl, items);
            }
            else
                res = await PostChunkInternalAsync(token, apiUrl, items, allItemsCount, chunkSize, continueOnChunkError, startIndex);
            return res;
        }
        public async Task<HttpResponseMessage> PostChunkAsync(TokenResponseModel token, string apiUrl, List<object> items, int chunkSize, bool continueOnChunkError)
        {
            if (items == null)
                throw new ArgumentException("items cannot be null!");
            var allItemsCount = items.Count;
            var startIndex = 0;
            HttpResponseMessage res = null;
            if (startIndex == 0 && allItemsCount < chunkSize)
            {
                res = await PostAsJsonAsync(token, apiUrl, items);
            }
            else
                res = await PostChunkInternalAsync(token, apiUrl, items, allItemsCount, chunkSize, continueOnChunkError, startIndex);
            return res;
        }
        public async Task<HttpResponseMessage> PostChunkInternalAsync<TObject>(TokenResponseModel token, string apiUrl, List<TObject> items, int allItemsCount, int chunkSize, bool continueOnChunkError, int startIndex)
        {
            HttpResponseMessage res = null;

            if (startIndex + chunkSize <= allItemsCount)
            {
                try
                {
                    var sentItems = items.Skip(startIndex).Take(chunkSize).ToList();
                    res = await PostAsJsonAsync(token, apiUrl, sentItems);
                }
                catch
                {
                    if (!continueOnChunkError)
                        throw;
                }
                startIndex += chunkSize;
                return await PostChunkInternalAsync(token, apiUrl, items, allItemsCount, chunkSize, continueOnChunkError, startIndex);
            }
            else
            {
                var remain = allItemsCount - startIndex;

                var sentItems = items.Skip(startIndex).Take(remain).ToList();
                res = await PostAsJsonAsync(token, apiUrl, sentItems);
                return res;
            }

        }
        public async Task<HttpResponseMessage> PostChunkInternalAsync(TokenResponseModel token, string apiUrl, List<object> items, int allItemsCount, int chunkSize, bool continueOnChunkError, int startIndex)
        {
            HttpResponseMessage res = null;

            if (startIndex + chunkSize <= allItemsCount)
            {
                try
                {
                    var sentItems = items.Skip(startIndex).Take(chunkSize).ToList();
                    res = await PostAsJsonAsync(token, apiUrl, sentItems);
                }
                catch
                {
                    if (!continueOnChunkError)
                        throw;
                }
                startIndex += chunkSize;
                return await PostChunkInternalAsync(token, apiUrl, items, allItemsCount, chunkSize, continueOnChunkError, startIndex);
            }
            else
            {
                var remain = allItemsCount - startIndex;

                var sentItems = items.Skip(startIndex).Take(remain).ToList();
                res = await PostAsJsonAsync(token, apiUrl, sentItems);
                return res;
            }

        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<TObject>(TokenResponseModel token, string apiUrl, TObject item)
        {
            return await PostAsJsonAsync(token, apiUrl, item, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> PostAsJsonAsync<TObject>(TokenResponseModel token, string apiUrl, TObject item, CancellationToken cancellationToken)
        {
            HttpResponseMessage postResponse = null;
            if (item == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(token, EnableCompression))
                {

                    postResponse = await client.PostAsJsonAsync(apiUrl, item, cancellationToken);
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var exception = postResponse.CreateApiException();
                        throw exception;

                    }
                    postResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = item, Error = null });
                    return postResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = item, Error = ex });
                throw;
            }
        }
        public async Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content)
        {
            return await PostAsync(token, apiUrl, content, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content, CancellationToken cancellationToken)
        {
            HttpResponseMessage postResponse = null;
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(token, EnableCompression))
                {

                    postResponse = await client.PostAsync(apiUrl, content, cancellationToken);
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var exception = postResponse.CreateApiException();
                        throw exception;

                    }
                    postResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = null });
                    return postResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content, string accept)
        {
            return await PostAsync(token, apiUrl, content, accept, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> PostAsync(TokenResponseModel token, string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken)
        {
            HttpResponseMessage postResponse = null;
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(token, EnableCompression, accept))
                {

                    postResponse = await client.PostAsync(apiUrl, content, cancellationToken);
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var exception = postResponse.CreateApiException();
                        throw exception;

                    }
                    postResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = null });
                    return postResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(TokenResponseModel token, string apiUrl, TSentObject item)
        {
            return await PostAsJsonAsync<TSentObject, TResultingObject>(token, apiUrl, item, CancellationToken.None);
        }
        public async Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(TokenResponseModel token, string apiUrl, TSentObject item, CancellationToken cancellationToken)
        {
            var postResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            if (item == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(token, EnableCompression))
                {

                    postResponse = await client.PostAsJsonAsync<TSentObject, TResultingObject>(apiUrl, item, cancellationToken);
                    if (!postResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = postResponse.Key.CreateApiException();
                        throw exception;

                    }
                    postResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Output = postResponse.Value, Item = item, Error = null });
                    return postResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Item = item, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content, string accept)
        {
            return await PostAsync<TResultingObject>(token, apiUrl, content, accept, CancellationToken.None);
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken)
        {
            var postResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(token, EnableCompression, accept))
                {

                    postResponse = await client.PostAsync<TResultingObject>(apiUrl, content, cancellationToken);
                    if (!postResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = postResponse.Key.CreateApiException();
                        throw exception;

                    }
                    postResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Output = postResponse.Value, Item = content, Error = null });
                    return postResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content)
        {
            return await PostAsync<TResultingObject>(token, apiUrl, content, CancellationToken.None);
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(TokenResponseModel token, string apiUrl, HttpContent content, CancellationToken cancellationToken)
        {
            var postResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(token, EnableCompression))
                {

                    postResponse = await client.PostAsync<TResultingObject>(apiUrl, content, cancellationToken);
                    if (!postResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = postResponse.Key.CreateApiException();
                        throw exception;

                    }
                    postResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Output = postResponse.Value, Item = content, Error = null });
                    return postResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content)
        {
            return await PostAsync(apiUrl, content, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content, CancellationToken cancellationToken)
        {
            HttpResponseMessage postResponse = null;
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(EnableCompression))
                {

                    postResponse = await client.PostAsync(apiUrl, content, cancellationToken);
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var exception = postResponse.CreateApiException();
                        throw exception;

                    }
                    postResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = null });
                    return postResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content, string accept)
        {
            return await PostAsync(apiUrl, content, accept, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> PostAsync(string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken)
        {
            HttpResponseMessage postResponse = null;
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(EnableCompression, accept))
                {

                    postResponse = await client.PostAsync(apiUrl, content, cancellationToken);
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var exception = postResponse.CreateApiException();
                        throw exception;

                    }
                    postResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = null });
                    return postResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<HttpResponseMessage> PostAsJsonAsync<TObject>(string apiUrl, TObject item)
        {
            return await PostAsJsonAsync(apiUrl, item, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> PostAsJsonAsync<TObject>(string apiUrl, TObject item, CancellationToken cancellationToken)
        {
            HttpResponseMessage postResponse = null;
            if (item == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(EnableCompression))
                {

                    postResponse = await client.PostAsJsonAsync(apiUrl, item, cancellationToken);
                    if (!postResponse.IsSuccessStatusCode)
                    {
                        var exception = postResponse.CreateApiException();
                        throw exception;

                    }
                    postResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = item, Error = null });
                    return postResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse, Item = item, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(string apiUrl, TSentObject item)
        {
            return await PostAsJsonAsync<TSentObject, TResultingObject>(apiUrl, item, CancellationToken.None);
        }
        public async Task<TResultingObject> PostAsJsonAsync<TSentObject, TResultingObject>(string apiUrl, TSentObject item, CancellationToken cancellationToken)
        {
            var postResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            if (item == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(EnableCompression))
                {

                    postResponse = await client.PostAsJsonAsync<TSentObject, TResultingObject>(apiUrl, item, cancellationToken);
                    if (!postResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = postResponse.Key.CreateApiException();
                        throw exception;

                    }
                    postResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Output = postResponse.Value, Item = item, Error = null });
                    return postResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Item = item, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content)
        {
            return await PostAsync<TResultingObject>(apiUrl, content, CancellationToken.None);
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content, CancellationToken cancellationToken)
        {
            var postResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(EnableCompression))
                {

                    postResponse = await client.PostAsync<TResultingObject>(apiUrl, content, cancellationToken);
                    if (!postResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = postResponse.Key.CreateApiException();
                        throw exception;

                    }
                    postResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Output = postResponse.Value, Item = content, Error = null });
                    return postResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Item = content, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content, string accept)
        {
            return await PostAsync<TResultingObject>(apiUrl, content, accept, CancellationToken.None);
        }
        public async Task<TResultingObject> PostAsync<TResultingObject>(string apiUrl, HttpContent content, string accept, CancellationToken cancellationToken)
        {
            var postResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            if (content == null)
                throw new ArgumentException("item cannot be null!");
            try
            {
                using (var client = GetClient(EnableCompression, accept))
                {

                    postResponse = await client.PostAsync<TResultingObject>(apiUrl, content, cancellationToken);
                    if (!postResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = postResponse.Key.CreateApiException();
                        throw exception;

                    }
                    postResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Output = postResponse.Value, Item = content, Error = null });
                    return postResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = postResponse.Key, Item = content, Error = ex });
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(TokenResponseModel token, string apiUrl)
        {
            return await GetAsync(token, apiUrl, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> GetAsync(TokenResponseModel token, string apiUrl, CancellationToken cancellationToken)
        {
            HttpResponseMessage getResponse = null;
            try
            {
                using (var client = GetClient(token, EnableCompression))
                {

                    getResponse = await client.GetAsync(apiUrl, cancellationToken);
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        var exception = getResponse.CreateApiException();
                        throw exception;

                    }
                    getResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse, Error = null });
                    return getResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> GetAsync<TResultingObject>(TokenResponseModel token, string apiUrl)
        {
            return await GetAsync<TResultingObject>(token, apiUrl, CancellationToken.None);
        }
        public async Task<TResultingObject> GetAsync<TResultingObject>(TokenResponseModel token, string apiUrl, CancellationToken cancellationToken)
        {
            var getResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            try
            {
                using (var client = GetClient(token, EnableCompression))
                {

                    getResponse = await client.GetAsync<TResultingObject>(apiUrl, cancellationToken);
                    if (!getResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = getResponse.Key.CreateApiException();
                        throw exception;

                    }
                    getResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse.Key, Output = getResponse.Value, Item = null, Error = null });
                    return getResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse.Key, Item = null, Error = ex });
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string apiUrl)
        {
            return await GetAsync(apiUrl, CancellationToken.None);
        }
        public async Task<HttpResponseMessage> GetAsync(string apiUrl, CancellationToken cancellationToken)
        {
            HttpResponseMessage getResponse = null;
            try
            {
                using (var client = GetClient(EnableCompression))
                {

                    getResponse = await client.GetAsync(apiUrl, cancellationToken);
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        var exception = getResponse.CreateApiException();
                        throw exception;

                    }
                    getResponse.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse, Error = null });
                    return getResponse;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse, Error = ex });
                throw;
            }
        }
        public async Task<TResultingObject> GetAsync<TResultingObject>(string apiUrl)
        {
            return await GetAsync<TResultingObject>(apiUrl, CancellationToken.None);
        }
        public async Task<TResultingObject> GetAsync<TResultingObject>(string apiUrl, CancellationToken cancellationToken)
        {
            var getResponse = new KeyValuePair<HttpResponseMessage, TResultingObject>();
            try
            {
                using (var client = GetClient(EnableCompression))
                {

                    getResponse = await client.GetAsync<TResultingObject>(apiUrl, cancellationToken);
                    if (!getResponse.Key.IsSuccessStatusCode)
                    {
                        var exception = getResponse.Key.CreateApiException();
                        throw exception;

                    }
                    getResponse.Key.EnsureSuccessStatusCode();
                    OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse.Key, Output = getResponse.Value, Item = null, Error = null });
                    return getResponse.Value;
                }
            }
            catch (Exception ex)
            {
                OnDataSendingCompleted(new DataSendingCompletedEventArgs() { Result = getResponse.Key, Item = null, Error = ex });
                throw;
            }
        }

        public async Task<TokenResponseModel> GetBearerTokenAsync(string username, string password)
        {
            if (_token == null || string.IsNullOrEmpty(_token.ExpiresAt))
                return await GetBearerTokenInternalAsync(username, password);
            else
            {
                var expDate = DateTime.Parse(_token.ExpiresAt, System.Globalization.DateTimeFormatInfo.InvariantInfo);
                if (DateTime.Now > expDate)
                    return await GetBearerTokenInternalAsync(username, password);
                else
                    return _token;
            }
        }
        public async Task<TokenResponseModel> GetBearerTokenInternalAsync(string username, string password)
        {

            using (var client = GetClient(false))
            {
                HttpContent requestContent = new StringContent("grant_type=password&username=" + username + "&password=" + password, Encoding.UTF8, "application/x-www-form-urlencoded");


                var responseMessage = await client.PostAsync("Token", requestContent);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string jsonMessage;
                    using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
                    {
                        jsonMessage = new StreamReader(responseStream).ReadToEnd();
                    }

                    var tokenResponse = (TokenResponseModel)JsonConvert.DeserializeObject(jsonMessage, typeof(TokenResponseModel));
                    _token = tokenResponse;
                    return _token;
                }
                else
                {

                    var exception = responseMessage.CreateApiException();
                    throw exception;


                }
            }
        }
        public virtual System.Net.Http.HttpClient GetClient()
        {
            return GetClient(EnableCompression, Accept);
        }
        public virtual System.Net.Http.HttpClient GetClient(string accept)
        {
            return GetClient(EnableCompression, accept);
        }
        public virtual System.Net.Http.HttpClient GetClient(TokenResponseModel token)
        {
            return GetClient(token, EnableCompression, Accept);
        }
        //public virtual System.Net.Http.HttpClient GetClient(TokenResponseModel token, string accept)
        //{
        //    return GetClient(token,this.EnableCompression, accept);
        //}
        public virtual System.Net.Http.HttpClient GetClient(bool enableCompression, string accept = "application/json")
        {
            System.Net.Http.HttpClient client;
            if (enableCompression)
            {
                client = new System.Net.Http.HttpClient(new ClientCompressionHandler(860, new GZipCompressor(), new DeflateCompressor()));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            }
            else
            {
                client = new System.Net.Http.HttpClient();
            }
            client.BaseAddress = new Uri(BaseAddress);
            if (accept != null)
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            if (timeoutInSeconds > 0)
                client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            return client;
        }

        public virtual System.Net.Http.HttpClient GetClient(TokenResponseModel token, bool enableCompression, string accept = "application/json")
        {
            var client = GetClient(enableCompression, accept);

            client.DefaultRequestHeaders.Accept.Clear();
            //افزودن توکن برای رمزگشایی و اعتبارسنجی در سمت سرور
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            //افزودن شناسه کاربری برای شناسایی کاربر در سمت سرور
            client.DefaultRequestHeaders.Add("UserId", token.UserId);
            return client;
        }
        public async Task<bool> CheckServerConnection()
        {
            var res = false;
            try
            {
                using (var client = GetClient())
                {
                    var httpres = await client.GetAsync("/");
                    res = httpres.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                res = false;
            }
            return res;
        }


    }
}