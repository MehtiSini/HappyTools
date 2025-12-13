using System.Diagnostics;
using HappyTools.Utilities.Extensions.HttpClient.CompressionHandler.Interface;
using HappyTools.Utilities.Extensions.HttpClient.CompressionHandler.Models;

namespace HappyTools.Utilities.Extensions.HttpClient.CompressionHandler
{
    //reference:https://github.com/azzlack/Microsoft.AspNet.WebApi.MessageHandlers.Compression
    /// <summary>
    /// Allow compressing web api response
    /// <example>
    /// <code>
    /// config.MessageHandlers.Insert(0, new CompressionHandler(new GZipCompressor(), new DeflateCompressor()));
    /// </code>
    /// By default, both ServerCompressionHandler and ClientCompressionHandler compress everything.
    /// However, this can be overriden by inserting a threshold as the first parameter like this:
    /// <code>
    /// var serverCompressionHandler = new ServerCompressionHandler(4096, new GZipCompressor(), new DeflateCompressor());
    /// var clientCompressionHandler = new ClientCompressionHandler(4096, new GZipCompressor(), new DeflateCompress
    /// </code>
    /// You need to apply the following code when creating your HttpClient.
    /// <code>
    /// var client = new HttpClient(new ClientompressionHandler(new GZipCompressor(), new DeflateCompressor()));
    /// client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
    /// client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
    /// </code>
    /// </example>
    /// </summary>
    public class CompressionHandler : DelegatingHandler
    {
        /// <summary>
        /// The content size threshold before compressing.
        /// </summary>
        private readonly int contentSizeThreshold;

        /// <summary>
        /// The HTTP content operations
        /// </summary>
        private readonly HttpContentOperations httpContentOperations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCompressionHandler" /> class.
        /// </summary>
        /// <param name="compressors">The compressors.</param>
        public CompressionHandler(params ICompressor[] compressors)
            : this(null, 860, compressors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCompressionHandler" /> class.
        /// </summary>
        /// <param name="contentSizeThreshold">The content size threshold before compressing.</param>
        /// <param name="compressors">The compressors.</param>
        public CompressionHandler(int contentSizeThreshold, params ICompressor[] compressors)
            : this(null, contentSizeThreshold, compressors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCompressionHandler" /> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler.</param>
        /// <param name="compressors">The compressors.</param>
        public CompressionHandler(HttpMessageHandler innerHandler, params ICompressor[] compressors)
            : this(innerHandler, 860, compressors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCompressionHandler" /> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler.</param>
        /// <param name="contentSizeThreshold">The content size threshold before compressing.</param>
        /// <param name="compressors">The compressors.</param>
        public CompressionHandler(HttpMessageHandler innerHandler, int contentSizeThreshold, params ICompressor[] compressors)
        {
            if (innerHandler != null)
            {
                InnerHandler = innerHandler;
            }

            Compressors = compressors;
            this.contentSizeThreshold = contentSizeThreshold;

            httpContentOperations = new HttpContentOperations();
        }

        /// <summary>
        /// Gets the compressors.
        /// </summary>
        /// <value>The compressors.</value>
        public ICollection<ICompressor> Compressors { get; private set; }

        /// <summary>
        /// send as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Decompress compressed requests to the server
            if (request.Content != null && request.Content.Headers.ContentEncoding.Any())
            {
                await DecompressRequest(request);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var process = true;

            try
            {
                // Buffer content for further processing
                await response.Content.LoadIntoBufferAsync();
            }
            catch (Exception ex)
            {
                process = false;

                Debug.WriteLine(ex.Message);
            }

            // Compress uncompressed responses from the server
            if (process && response.Content != null && request.Headers.AcceptEncoding.Any())
            {
                await CompressResponse(request, response);
            }

            return response;
        }

        /// <summary>
        /// Compresses the content.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <returns>An async void.</returns>
        public async Task CompressResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            // As per RFC2616.14.3:
            // Ignores encodings with quality == 0
            // If multiple content-codings are acceptable, then the acceptable content-coding with the highest non-zero qvalue is preferred.
            var compressor = (from encoding in request.Headers.AcceptEncoding
                              let quality = encoding.Quality ?? 1.0
                              where quality > 0
                              join c in Compressors on encoding.Value.ToLowerInvariant() equals
                                  c.EncodingType.ToLowerInvariant()
                              orderby quality descending
                              select c).FirstOrDefault();

            if (compressor != null)
            {
                try
                {
                    // Only compress response if size is larger than treshold (if set)
                    if (contentSizeThreshold == 0)
                    {
                        response.Content = new CompressedContent(response.Content, compressor);
                    }
                    else if (contentSizeThreshold > 0 && (response.Content.Headers.ContentLength == null || response.Content.Headers.ContentLength >= contentSizeThreshold))
                    {
                        response.Content = new CompressedContent(response.Content, compressor);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to compress response using compressor '{0}'", compressor.GetType()), ex);
                }
            }
        }

        /// <summary>
        /// Decompresses the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An async void.</returns>
        public async Task DecompressRequest(HttpRequestMessage request)
        {
            var encoding = request.Content.Headers.ContentEncoding.First();

            var compressor = Compressors.FirstOrDefault(c => c.EncodingType.Equals(encoding, StringComparison.OrdinalIgnoreCase));

            if (compressor != null)
            {
                try
                {
                    request.Content = await httpContentOperations.DecompressContent(request.Content, compressor).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to decompress request using compressor '{0}'", compressor.GetType()), ex);
                }
            }
        }
    }
}