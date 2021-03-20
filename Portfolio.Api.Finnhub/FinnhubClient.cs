using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Portfolio.Api.Finnhub
{
    public partial class FinnhubClient
    {
        private readonly string _token;

        public FinnhubClient(HttpClient httpClient, string token) : this(httpClient)
        {
            this._token = token;
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, System.Text.StringBuilder urlBuilder)
        {
            // add authentication token to query
            urlBuilder.Append("&").Append(System.Uri.EscapeDataString("token") + "=").Append(System.Uri.EscapeDataString(ConvertToString(_token, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
        }
    }
}

