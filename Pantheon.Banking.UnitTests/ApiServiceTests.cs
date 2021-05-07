using System;
using Xunit;
using NSubstitute;
using Pantheon.Banking.Service;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Pantheon.Banking.Domain;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace Pantheon.Banking.UnitTests
{
    public class ApiServiceTests
    {
        [Fact]
        public void Test_ExchangeRate_Api_Return_CorrectData()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var httpClient = new HttpClient(new FakeHttpMessageHandler()) { BaseAddress = new Uri("https://localhost") };
            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            var apiService = new ApiService(httpClientFactory);
            var res = apiService.Get<ExchangeRate>("https://localhost").Result;

            res.Base.ShouldBe("USD");
            res.Rates.FirstOrDefault().Value.ShouldBe(0.71);
            res.Rates.FirstOrDefault().Key.ShouldBe("GBP");
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"base\":\"USD\",\"rates\":{\"GBP\":0.71},\"date\":\"2021-05-07\"}")
            });
        }
    }
}
