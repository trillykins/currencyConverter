using CurrencyConverter;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverterTestProject
{
    [TestClass]
    public class UnitTest1
    {
        private MockRepository _mockRepository;
        private Conversion _conversion;
        private Mock<HttpMessageHandler> _handlerMock;
        private HttpClient _magicHttpClient;

        [TestInitialize]
        public void init()
        {
            // Mock IConfiguration
            var appSettingsStub = new Dictionary<string, string>
            {
                {"ConnectionStrings:freeCurrencyApi", "https://freecurrencyapi.net"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(appSettingsStub)
                .Build();

            // Mock HttpResponseMessage (we want to test the implementation and not the endpoint it's dependent on)
            _mockRepository = new MockRepository(MockBehavior.Default);
            _handlerMock = _mockRepository.Create<HttpMessageHandler>();
            _magicHttpClient = new HttpClient(_handlerMock.Object);
            _conversion = new Conversion(configuration, _magicHttpClient);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText(@"test-data.json")),
            };
            _handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
        }

        [TestMethod]
        public async Task TestMethod1Async()
        {
            var result = await _conversion.FetchAllAvailableCurrencies();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public async Task TestMethod2Async()
        {
            var result = await _conversion.ConvertCurrencyAsync("USD", "JPY", 1200);
            Assert.AreEqual(137935, (int)result);
        }

        [TestMethod]
        public async Task TestMethod3Async()
        {
            try
            {
                await _conversion.ConvertCurrencyAsync("MS", "DOS", 1200);
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("The parameter currencyFrom, MS, is not a valid currency", e.Message);
            }
        }

        [TestMethod]
        public async Task TestMethod4Async()
        {
            try
            {
                await _conversion.ConvertCurrencyAsync("USD", "DOS", 1200);
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("The parameter currencyTo, DOS, is not a valid currency", e.Message);
            }
        }

        [TestMethod]
        public async Task TestMethod5Async()
        {
            try
            {
                await _conversion.ConvertCurrencyAsync(null, "JPY", 1200);
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("Value of currencyFrom cannot be null", e.Message);
            }
        }
        [TestMethod]
        public async Task TestMethod6Async()
        {
            try
            {
                await _conversion.ConvertCurrencyAsync("USD", null, 1200);
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("Value of currencyTo cannot be null", e.Message);
            }
        }

        [TestMethod]
        public async Task TestMethod7Async()
        {
            try
            {
                await _conversion.ConvertCurrencyAsync("USD", "DOS", -1200);
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("Value of value '-1200' is not valid!", e.Message);
            }
        }
    }
}
