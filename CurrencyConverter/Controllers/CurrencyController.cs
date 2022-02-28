using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly Conversion _conversion;

        public CurrencyController(ILogger<CurrencyController> logger, Conversion conversion)
        {
            _logger = logger;
            _conversion = conversion;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            return await _conversion.FetchAllAvailableCurrencies();
        }

        [HttpGet]
        [Route("convert")]
        public async Task<double> GetAsync(string currencyFrom, string currencyTo, double value)
        {
            // https://localhost:44379/currency/convert?currencyFrom=DKK&currencyTo=JPY&value=1200
            return await _conversion.ConvertCurrencyAsync(currencyFrom, currencyTo, value);
        }
    }
}
