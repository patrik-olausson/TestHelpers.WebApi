using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestHelpers.WebApi
{
    internal static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            return httpClient.PostAsync(requestUri, CreateJsonContent(value));
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            return httpClient.PutAsync(requestUri, CreateJsonContent(value));
        }

        private static HttpContent CreateJsonContent<T>(T value, bool indented = true, Encoding encoding = null)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = indented ? Formatting.Indented : Formatting.None
            };
            var jsonString = JsonConvert.SerializeObject(value, settings);

            return new StringContent(jsonString, encoding ?? Encoding.UTF8, "application/json");
        }
    }
}