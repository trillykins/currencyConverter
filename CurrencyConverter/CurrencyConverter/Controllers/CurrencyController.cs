using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
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
        public async Task<ActionResult> GetAsync()
        {
            return new ObjectResult(await _conversion.FetchAllAvailableCurrencies());
        }

        [HttpGet]
        [Route("convert")]
        public async Task<ActionResult> GetAsync(string currencyFrom, string currencyTo, double value)
        {
            // https://localhost:44379/currency/convert?currencyFrom=DKK&currencyTo=JPY&value=1200
            if (double.IsNegative(value) || !double.IsNormal(value)) throw new Exception($"Value of {nameof(value)} '{value}' is not valid!");
            if (currencyFrom == null) throw new Exception($"Value of {nameof(currencyFrom)} cannot be null");
            if (currencyTo == null) throw new Exception($"Value of {nameof(currencyTo)} cannot be null");
            return await _conversion.ConvertCurrencyAsync(currencyFrom, currencyTo, value);
        }
    }
}
