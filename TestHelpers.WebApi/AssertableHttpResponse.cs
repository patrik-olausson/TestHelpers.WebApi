using System;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestHelpers.Utils;

namespace TestHelpers.WebApi
{
    public class AssertableHttpResponse
    {
        public string Body { get; }

        public string BodyAsJsonFormattedString
        {
            get
            {
                if (Body.LooksLikeItContainsJson())
                    return JToken.Parse(Body).ToString(Formatting.Indented);

                return "{}";
            }
        }
        public HttpStatusCode StatusCode { get; }
        public HttpResponseHeaders Headers { get; }

        public AssertableHttpResponse(
            HttpStatusCode statusCode,
            string body,
            HttpResponseHeaders responseHeaders)
        {
            StatusCode = statusCode;
            Headers = responseHeaders;
            Body = body;
        }

        /// <summary>
        /// Throws an exception if the status code isn't within the 2xx range
        /// </summary>
        public void EnsureSuccessStatusCode()
        {
            var numericStatusCode = (int)StatusCode;
            if (numericStatusCode >= 200 && numericStatusCode < 300)
                return;

            throw new Exception($"Status code {StatusCode} is not considered as success.\nBody: {Body}");
        }

        public override string ToString()
        {
            return $"StatusCode: {StatusCode}\nHeaders: {Headers?.ToJsonString()}\nBody: {BodyAsJsonFormattedString}";
        }
    }
}