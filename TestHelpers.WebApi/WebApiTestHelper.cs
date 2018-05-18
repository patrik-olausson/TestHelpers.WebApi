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
        private readonly Lazy<TestServer> _owinTestServer;
        protected HttpClient HttpClient => _owinTestServer.Value.HttpClient;

        protected WebApiTestHelper(Action<string> testOutput)
        {
            _testOutput = testOutput;
            _owinTestServer = new Lazy<TestServer>(() => TestServer.Create(ConfigureApp));
        }

        protected abstract void ConfigureApp(IAppBuilder appBuilder);

        public virtual async Task<AssertableHttpResponse> GetAsync(string requestUri, bool ensureSuccessStatusCode = true)
        {
            var response = await HttpClient.GetAsync(requestUri);
            return await CreateAssertableResponseAsync(response, $"GET {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PostAsJsonAsync<T>(string requestUri, T value, bool ensureSuccessStatusCode = true)
        {
            var response = await HttpClient.PostAsJsonAsync(requestUri, value);
            return await CreateAssertableResponseAsync(response, $"POST {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PutAsJsonAsync<T>(string requestUri, T value, bool ensureSuccessStatusCode = true)
        {
            var response = await HttpClient.PutAsJsonAsync(requestUri, value);
            return await CreateAssertableResponseAsync(response, $"PUT {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> DeleteAsync(string requestUri, bool ensureSuccessStatusCode = true)
        {
            var response = await HttpClient.DeleteAsync(requestUri);
            return await CreateAssertableResponseAsync(response, $"DELETE {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> OptionsAsync<T>(string requestUri, T value, bool ensureSuccessStatusCode = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Options, requestUri);
            var response = await HttpClient.SendAsync(request);
            return await CreateAssertableResponseAsync(response, $"OPTIONS {requestUri}", ensureSuccessStatusCode);
        }

        public virtual async Task<AssertableHttpResponse> PatchAsync<T>(string requestUri, T value, bool ensureSuccessStatusCode = true)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri);
            var response = await HttpClient.SendAsync(request);
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
