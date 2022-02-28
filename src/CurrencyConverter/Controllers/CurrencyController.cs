using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly Conversion _conversion;

        public CurrencyController(Conversion conversion)
        {
            _conversion = conversion;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return new ObjectResult(await _conversion.FetchAllAvailableCurrencies());
        }

        [HttpGet]
        [Route("convert")]
        public async Task<IActionResult> GetAsync(string currencyFrom, string currencyTo, double value)
        {
            // https://localhost:44379/currency/convert?currencyFrom=DKK&currencyTo=JPY&value=1200
            return new ObjectResult(await _conversion.ConvertCurrencyAsync(currencyFrom, currencyTo, value));
        }
    }
}
