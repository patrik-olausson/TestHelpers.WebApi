using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Owin;

namespace TestHelpers.WebApi
{
    public abstract class WebApiTestHelper : IDisposable
    {
        private readonly Action<string> _testOutput;
        private readonly Action<HttpClient> _configureHttpClientAction;
        private readonly Lazy<TestServer> _owinTestServer;
        protected HttpClient HttpClient => _owinTestServer.Value.HttpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testOutput"></param>
        /// <param name="configureHttpClientAction">Configuration you want to perform before every call</param>
        protected WebApiTestHelper(
            Action<string> testOutput, 
            Action<HttpClient> configureHttpClientAction = null)
        {
            _testOutput = testOutput;
            _configureHttpClientAction = configureHttpClientAction;
            _owinTestServer = new Lazy<TestServer>(() => TestServer.Create(ConfigureApp));
        }

        protected abstract void ConfigureApp(IAppBuilder appBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="ensureSuccessStatusCode"></param>
        /// <param name="preRequestConfigureHttpClientAction">Configuration you want to perform before this call (if you have 
        /// specified an configuration action in the constructor it will also be called!)</param>
        /// <returns></returns>
        public virtual async Task<AssertableHttpResponse> GetAsync(
            string requestUri, 
            bool ensureSuccessStatusCode = true, 
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            var httpClient = HttpClient;
            _configureHttpClientAction?.Invoke(httpClient);
            preRequestConfigureHttpClientAction?.Invoke(httpClient);

            var response = await httpClient.GetAsync(requestUri);

            return await CreateAssertableResponseAsync(response, $"GET {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PostAsJsonAsync<T>(
            string requestUri, 
            T value, bool ensureSuccessStatusCode = true, 
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            var httpClient = HttpClient;
            _configureHttpClientAction?.Invoke(httpClient);
            preRequestConfigureHttpClientAction?.Invoke(httpClient);

            var response = await httpClient.PostAsJsonAsync(requestUri, value);

            return await CreateAssertableResponseAsync(response, $"POST {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PutAsJsonAsync<T>(
            string requestUri,
            T value, 
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            var httpClient = HttpClient;
            _configureHttpClientAction?.Invoke(httpClient);
            preRequestConfigureHttpClientAction?.Invoke(httpClient);

            var response = await httpClient.PutAsJsonAsync(requestUri, value);

            return await CreateAssertableResponseAsync(response, $"PUT {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> DeleteAsync(
            string requestUri, 
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            var httpClient = HttpClient;
            _configureHttpClientAction?.Invoke(httpClient);
            preRequestConfigureHttpClientAction?.Invoke(httpClient);

            var response = await httpClient.DeleteAsync(requestUri);

            return await CreateAssertableResponseAsync(response, $"DELETE {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> OptionsAsync<T>(
            string requestUri,
            T value, 
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            var httpClient = HttpClient;
            _configureHttpClientAction?.Invoke(httpClient);
            preRequestConfigureHttpClientAction?.Invoke(httpClient);
            var request = new HttpRequestMessage(HttpMethod.Options, requestUri);

            var response = await httpClient.SendAsync(request);
            return await CreateAssertableResponseAsync(response, $"OPTIONS {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PatchAsync<T>(
            string requestUri, 
            T value, 
            bool ensureSuccessStatusCode = true,
            Action<HttpClient> preRequestConfigureHttpClientAction = null)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri);
            var httpClient = HttpClient;
            _configureHttpClientAction?.Invoke(httpClient);
            preRequestConfigureHttpClientAction?.Invoke(httpClient);

            var response = await httpClient.SendAsync(request);

            return await CreateAssertableResponseAsync(response, $"PATCH {requestUri}", ensureSuccessStatusCode);
        }

        private async Task<AssertableHttpResponse> CreateAssertableResponseAsync(
            HttpResponseMessage response,
            string testOutput,
            bool ensureSuccessStatusCode)
        {
            var assertableResponse = new AssertableHttpResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync(),
                response.Headers);

            OutputToTestLog($"{testOutput}\nResponse:\n{assertableResponse}");

            if (ensureSuccessStatusCode)
                assertableResponse.EnsureSuccessStatusCode();

            return assertableResponse;
        }

        private void OutputToTestLog(string message)
        {
            _testOutput?.Invoke($"\n{message}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_owinTestServer.IsValueCreated)
                {
                    _owinTestServer.Value.Dispose();
                }
            }
        }
    }
}
