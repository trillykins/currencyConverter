using CurrencyConverter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IActionResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromQuery] Currency convertion)
        {
            try
            {
                // https://localhost:44379/currency/convert?currencyFrom=DKK&currencyTo=JPY&value=1200
                return Ok(await _conversion.ConvertCurrencyAsync(convertion));
            }
            catch (KeyNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
