using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;

namespace Portfolio.Api.Finnhub
{
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "11.5.1.0")]
    public class FinnhubApiException : Exception
    {
        public FinnhubApiException() : base() { }
        public FinnhubApiException(string message) : base(message) { }
        public FinnhubApiException(string message, Exception innerException) : base(message, innerException) { }

        // Do NOT make FinnhubApiException<Error> an inner exception.  Only cary over the data
        public FinnhubApiException(string message, FinnhubApiException exception) : base(message)
        {
            this.Hydrate(exception);
        }
        public void Hydrate(FinnhubApiException value)
        {
            this.StatusCode = value.StatusCode;
            this.Response = value.Response;
            this.Headers = value.Headers;
        }

        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public FinnhubApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "11.5.1.0")]
    public class FinnhubApiException<TResult> : FinnhubApiException
    {
        public TResult Result { get; private set; }

        public FinnhubApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }
}