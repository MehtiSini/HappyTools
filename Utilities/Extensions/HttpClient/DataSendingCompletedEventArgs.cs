using System;
using System.Net.Http;

namespace HappyTools.Utilities.Extensions.HttpClient
{
    public class DataSendingCompletedEventArgs : EventArgs
    {
        private HttpResponseMessage result;
        private object item;
        private Exception error;

        public Exception Error {
            get {
                return error;
            }
            set {
                error = value;
            }
        }
        public bool HasError {
            get {
                return Error != null;
            }
        }

        public HttpResponseMessage Result {
            get {
                return result;
            }
            set {
                result = value;
            }
        }
        public object Output {
            get;
            set;
        }
        public object Item {
            get {
                return item;
            }
            set {
                item = value;
            }
        }
    }
}