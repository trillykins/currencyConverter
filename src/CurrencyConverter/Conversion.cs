using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter
{
    public class Conversion
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public Conversion(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<IEnumerable<string>> FetchAllAvailableCurrencies()
        {
            var currencies = await FetchCurrencies();
            List<string> list = currencies.Data.Keys.ToList();
            list.Add(currencies.Query.BaseCurrency);
            list.Sort();
            return list.Distinct();
        }

        public async Task<CurrencyConversionResponse> ConvertCurrencyAsync(string currencyFrom, string currencyTo, double value)
        {
            if (double.IsNegative(value) || !double.IsNormal(value)) throw new ArgumentException($"Value of {nameof(value)} '{value}' is not valid!");

            var currencies = await FetchCurrencies();
            var data = currencies.Data;
            if (!data.ContainsKey(currencies.Query.BaseCurrency)) data.Add(currencies.Query.BaseCurrency, 1f); // USD, base currency is always 1
            if (!data.ContainsKey(currencyFrom)) throw new ArgumentException($"The parameter {nameof(currencyFrom)}, {currencyFrom}, is not a valid currency");
            if (!data.ContainsKey(currencyTo)) throw new ArgumentException($"The parameter {nameof(currencyTo)}, {currencyTo}, is not a valid currency");

            return new CurrencyConversionResponse
            {
                ConvertedAmount = (data[currencyTo] / data[currencyFrom]) * value,
                CurrencyConverteredOrigin = currencyFrom,
                CurrencyConverteredTarget = currencyTo,
                OriginalAmount = value
            };
        }

        private async Task<CurrencyResponse> FetchCurrencies()
        {
            try
            {
                // Consider a caching solution to reduce amount of calls to freecurrency api
                var request = await _client.GetAsync(_configuration.GetConnectionString("freeCurrencyApi"));
                if (request.IsSuccessStatusCode)
                {
                    var content = await request.Content.ReadAsStringAsync();
                    var currencies = JsonConvert.DeserializeObject<CurrencyResponse>(content);
                    return currencies;
                }
                throw new Exception($"Failed to fetch currencies: {request.StatusCode}");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
