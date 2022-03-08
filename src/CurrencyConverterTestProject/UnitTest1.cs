using CurrencyConverter;
using CurrencyConverter.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private Currency _currency;

        [TestInitialize]
        public void Init()
        {
            _currency = new Currency
            {
                Amount = 1200f,
                CurrencyFrom = "USD",
                CurrencyTo = "JPY"
            };

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
                Content = new StringContent(File.ReadAllText(@"test-data.json")),   // Consider generating instead
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
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public async Task TestMethod2Async()
        {
            var result = await _conversion.ConvertCurrencyAsync(_currency);
            Assert.AreEqual("USD", result.CurrencyConverteredOrigin);
            Assert.AreEqual("JPY", result.CurrencyConverteredTarget);
            Assert.AreEqual(1200, (int)result.OriginalAmount);
            Assert.AreEqual(137935, (int)result.ConvertedAmount);
        }

        [TestMethod]
        public async Task TestMethod3Async()
        {
            _currency.CurrencyFrom = "MS";
            try
            {
                await _conversion.ConvertCurrencyAsync(_currency);
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is KeyNotFoundException);
                Assert.AreEqual("The parameter CurrencyFrom, MS, is not a valid currency", e.Message);
            }
        }

        [TestMethod]
        public async Task TestMethod4Async()
        {
            _currency.CurrencyTo = "DOS";
            try
            {
                await _conversion.ConvertCurrencyAsync(_currency); 
                Assert.Fail("Supposed to fail test-case");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is KeyNotFoundException);
                Assert.AreEqual("The parameter CurrencyTo, DOS, is not a valid currency", e.Message);
            }
        }

        //[TestMethod]
        //public async Task TestMethod5Async()
        //{
        //    _currency.CurrencyFrom = null;

        //    try
        //    {
        //        var result = await _conversion.ConvertCurrencyAsync(_currency);
        //        Assert.Fail("Supposed to fail test-case");
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.IsTrue(e is ArgumentNullException);
        //        Assert.AreEqual("Value of CurrencyFrom cannot be null", e.Message);
        //    }
        //}
        //[TestMethod]
        //public async Task TestMethod6Async()
        //{
        //    _currency.CurrencyTo = null;
        //    try
        //    {
        //        await _conversion.ConvertCurrencyAsync(_currency);
        //        Assert.Fail("Supposed to fail test-case");
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.IsTrue(e is ArgumentNullException);
        //        Assert.AreEqual("Value of CurrencyTo cannot be null", e.Message);
        //    }
        //}

        //[TestMethod]
        //public async Task TestMethod7Async()
        //{
        //    _currency.Amount = -1200f;
        //    try
        //    {
        //        var result = await _conversion.ConvertCurrencyAsync(_currency);
        //        Assert.Fail("Supposed to fail test-case");
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.IsTrue(e is KeyNotFoundException);
        //        Assert.AreEqual("Value of Amount '-1200' is not valid!", e.Message);
        //    }
        //}
    }
}
