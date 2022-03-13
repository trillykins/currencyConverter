using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
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
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _conversion.FetchAllAvailableCurrencies(cancellationToken));
            }
            catch (Exception)
            {
                return NotFound("Service is temporarily unavailable");
            }
        }

        [HttpGet]
        [Route("convert")]
        public async Task<IActionResult> GetAsync([FromQuery] Currency currency, CancellationToken cancellationToken)
        {
            try
            {
                // https://localhost:44379/currency/convert?currencyFrom=DKK&currencyTo=JPY&amount=1200
                return Ok(await _conversion.ConvertCurrencyAsync(currency.CurrencyFrom, currency.CurrencyTo, currency.Amount, cancellationToken));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return NotFound("Service is temporarily unavailable");
            }
        }
    }
}
