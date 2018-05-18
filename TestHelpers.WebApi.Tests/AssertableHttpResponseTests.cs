using System;
using System.Net;
using System.Net.Http.Headers;
using TestHelpers.WebApi;
using Xunit;

namespace AssertableHttpResponseTests
{
    public class EnsureSuccessStatusCode
    {
        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.NoContent)]
        public void GivenThatTheStatusCodeIndicatesSuccess_ThenItDowsNotThrowAnException(HttpStatusCode statusCode)
        {
            var sut = CreateSut(statusCode);

            sut.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(HttpStatusCode.Moved)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public void GivenThatTheStatusCodeIndicatesFailure_ThenItThrowsAnExceptionWithInformationAboutNotBeingSuccesAndWhatTheStatusCodeIs(HttpStatusCode statusCode)
        {
            var sut = CreateSut(statusCode);

            var exception = Assert.Throws<Exception>(() => sut.EnsureSuccessStatusCode());

            Assert.Contains(statusCode.ToString(), exception.Message);
        }

        private AssertableHttpResponse CreateSut(
            HttpStatusCode? statusCode = null,
            string body = "DefaultBody",
            HttpResponseHeaders responseHeaders = null)
        {
            return new AssertableHttpResponse(
                statusCode:statusCode.GetValueOrDefault(HttpStatusCode.OK),
                body: body,
                responseHeaders: responseHeaders);
        }
    }
}
