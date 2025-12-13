using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HappyTools.Utilities.Extensions.HttpClient.HttpError
{
    /// <summary>Defines a serializable container for storing error information. This information is stored  as key/value pairs. The dictionary keys to look up standard error information are available on the <see cref="T:BUF.CrossPlatform.CompactFramework.HttpClient.HttpError.HttpErrorKeys" /> type.</summary>
    public sealed class HttpError : Dictionary<string, object>, IXmlSerializable
    {
        /// <summary>Gets or sets the high-level, user-visible message explaining the cause of the error. Information carried in this field should be considered public in that it will go over the wire regardless of the <see cref="T:System.Web.Http.IncludeErrorDetailPolicy" />. As a result care should be taken not to disclose sensitive information about the server or the application.</summary>
        /// <returns>The high-level, user-visible message explaining the cause of the error. Information carried in this field should be considered public in that it will go over the wire regardless of the <see cref="T:System.Web.Http.IncludeErrorDetailPolicy" />. As a result care should be taken not to disclose sensitive information about the server or the application.</returns>
        public string Message {
            get {
                return GetPropertyValue<string>(HttpErrorKeys.MessageKey);
            }
            set {
                this[HttpErrorKeys.MessageKey] = value;
            }
        }

        /// <summary>Gets the <see cref="P:Soodane.Shared.Library.HttpClient.HttpError.HttpError.ModelState" /> containing information about the errors that occurred during model binding.</summary>
        /// <returns>The <see cref="P:Soodane.Shared.Library.HttpClient.HttpError.HttpError.ModelState" /> containing information about the errors that occurred during model binding.</returns>
        public HttpError ModelState {
            get {
                return GetPropertyValue<HttpError>(HttpErrorKeys.ModelStateKey);
            }
        }

        /// <summary>Gets or sets a detailed description of the error intended for the developer to understand exactly what failed.</summary>
        /// <returns>A detailed description of the error intended for the developer to understand exactly what failed.</returns>
        public string MessageDetail {
            get {
                return GetPropertyValue<string>(HttpErrorKeys.MessageDetailKey);
            }
            set {
                this[HttpErrorKeys.MessageDetailKey] = value;
            }
        }

        /// <summary>Gets or sets the message of the <see cref="T:System.Exception" /> if available.</summary>
        /// <returns>The message of the <see cref="T:System.Exception" /> if available.</returns>
        public string ExceptionMessage {
            get {
                return GetPropertyValue<string>(HttpErrorKeys.ExceptionMessageKey);
            }
            set {
                this[HttpErrorKeys.ExceptionMessageKey] = value;
            }
        }

        /// <summary>Gets or sets the type of the <see cref="T:System.Exception" /> if available.</summary>
        /// <returns>The type of the <see cref="T:System.Exception" /> if available.</returns>
        public string ExceptionType {
            get {
                return GetPropertyValue<string>(HttpErrorKeys.ExceptionTypeKey);
            }
            set {
                this[HttpErrorKeys.ExceptionTypeKey] = value;
            }
        }

        /// <summary>Gets or sets the stack trace information associated with this instance if available.</summary>
        /// <returns>The stack trace information associated with this instance if available.</returns>
        public string StackTrace {
            get {
                return GetPropertyValue<string>(HttpErrorKeys.StackTraceKey);
            }
            set {
                this[HttpErrorKeys.StackTraceKey] = value;
            }
        }

        /// <summary>Gets the inner <see cref="T:System.Exception" /> associated with this instance if available.</summary>
        /// <returns>The inner <see cref="T:System.Exception" /> associated with this instance if available.</returns>
        public HttpError InnerException {
            get {
                return GetPropertyValue<HttpError>(HttpErrorKeys.InnerExceptionKey);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:BUF.CrossPlatform.CompactFramework.HttpClient.HttpError.HttpError" /> class.</summary>
        public HttpError()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:BUF.CrossPlatform.CompactFramework.HttpClient.HttpError.HttpError" /> class containing error message <paramref name="message" />.</summary>
        /// <param name="message">The error message to associate with this instance.</param>
        public HttpError(string message)
            : this()
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            Message = message;
        }

        /// <summary>Initializes a new instance of the <see cref="T:BUF.CrossPlatform.CompactFramework.HttpClient.HttpError.HttpError" /> class for <paramref name="exception" />.</summary>
        /// <param name="exception">The exception to use for error information.</param>
        /// <param name="includeErrorDetail">true to include the exception information in the error; false otherwise</param>
        public HttpError(Exception exception, bool includeErrorDetail)
            : this()
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            Message = "An error has been occured!";
            if (!includeErrorDetail)
                return;
            Add(HttpErrorKeys.ExceptionMessageKey, exception.Message);
            Add(HttpErrorKeys.ExceptionTypeKey, exception.GetType().FullName);
            Add(HttpErrorKeys.StackTraceKey, exception.StackTrace);
            if (exception.InnerException == null)
                return;
            Add(HttpErrorKeys.InnerExceptionKey, new HttpError(exception.InnerException, includeErrorDetail));
        }

        /// <summary>Initializes a new instance of the <see cref="T:BUF.CrossPlatform.CompactFramework.HttpClient.HttpError.HttpError" /> class for <paramref name="modelState" />.</summary>
        /// <param name="modelState">The invalid model state to use for error information.</param>
        /// <param name="includeErrorDetail">true to include exception messages in the error; false otherwise</param>
        public HttpError(ModelStateDictionary modelState, bool includeErrorDetail)
            : this()
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));
            if (modelState.IsValid)
                throw new ArgumentException("ModelState is not valid!", nameof(modelState));
            Message = "Bad Request";
            var httpError = new HttpError();
            foreach (var keyValuePair in modelState)
            {
                var key = keyValuePair.Key;
                var errors = keyValuePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    var array = (IEnumerable<string>)errors.Select(error =>
                    {
                        if (includeErrorDetail && error.Exception != null)
                            return error.Exception.Message;
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                            return error.ErrorMessage;
                        return "An error has been occured!";
                    }).ToArray();
                    httpError.Add(key, array);
                }
            }
            Add(HttpErrorKeys.ModelStateKey, httpError);
        }

        internal HttpError(string message, string messageDetail)
            : this(message)
        {
            if (messageDetail == null)
                throw new ArgumentNullException("message");
            Add(HttpErrorKeys.MessageDetailKey, messageDetail);
        }

        /// <summary>Gets a particular property value from this error instance.</summary>
        /// <returns>A particular property value from this error instance.</returns>
        /// <param name="key">The name of the error property.</param>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        public TValue GetPropertyValue<TValue>(string key)
        {
            object obj;
            if (TryGetValue(key, out obj))
                return (TValue)obj;
            return default;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.ReadStartElement();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    Add(XmlConvert.DecodeName(reader.LocalName), reader.ReadInnerXml());
                    var content = (int)reader.MoveToContent();
                }
                reader.ReadEndElement();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var keyValuePair in (Dictionary<string, object>)this)
            {
                var key = keyValuePair.Key;
                var obj = keyValuePair.Value;
                writer.WriteStartElement(XmlConvert.EncodeLocalName(key));
                if (obj != null)
                {
                    var httpError = obj as HttpError;
                    if (httpError == null)
                        writer.WriteValue(obj);
                    else
                        ((IXmlSerializable)httpError).WriteXml(writer);
                }
                writer.WriteEndElement();
            }
        }
    }
    public static class HttpErrorKeys
    {
        /// <summary> Provides a key for the Message. </summary>
        public static readonly string MessageKey = "Message";
        /// <summary> Provides a key for the MessageDetail. </summary>
        public static readonly string MessageDetailKey = "MessageDetail";
        /// <summary> Provides a key for the ModelState. </summary>
        public static readonly string ModelStateKey = "ModelState";
        /// <summary> Provides a key for the ExceptionMessage. </summary>
        public static readonly string ExceptionMessageKey = "ExceptionMessage";
        /// <summary> Provides a key for the ExceptionType. </summary>
        public static readonly string ExceptionTypeKey = "ExceptionType";
        /// <summary> Provides a key for the StackTrace. </summary>
        public static readonly string StackTraceKey = "StackTrace";
        /// <summary> Provides a key for the InnerException. </summary>
        public static readonly string InnerExceptionKey = "InnerException";
        /// <summary> Provides a key for the MessageLanguage. </summary>
        public static readonly string MessageLanguageKey = "MessageLanguage";
        /// <summary> Provides a key for the ErrorCode. </summary>
        public static readonly string ErrorCodeKey = "ErrorCode";
    }
}
