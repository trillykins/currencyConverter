using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter
{
    public class Conversion
    {
        static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;

        public Conversion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string[]> FetchAllAvailableCurrencies()
        {
            var currencies = await FetchCurrencies();
            List<string> list = currencies.Data.Keys.ToList();
            list.Add(currencies.Query.BaseCurrency);
            list.Sort();
            list.Distinct();
            return list.ToArray();
        }

        public async Task<ActionResult> ConvertCurrencyAsync(string currencyFrom, string currencyTo, double value)
        {
            var currencies = await FetchCurrencies();
            var data = currencies.Data;
            if (!data.ContainsKey(currencyFrom)) throw new Exception($"The parameter {nameof(currencyFrom)}, {currencyFrom}, is not a valid currency");
            if (data.ContainsKey(currencyTo)) throw new Exception($"The parameter {nameof(currencyTo)}, {currencyTo}, is not a valid currency");

            return new ObjectResult((data[currencyTo] / data[currencyFrom]) * value);
        }

        private async Task<CurrencyResponse> FetchCurrencies()
        {
            // Consider a caching solution to reduce amount of calls to freecurrency api
            var request = await client.GetAsync(_configuration.GetConnectionString("freeCurrencyApi"));
            if (request.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStringAsync();
                var currencies = JsonConvert.DeserializeObject<CurrencyResponse>(content);
                return currencies;
            }
            throw new Exception("Failed to fetch currencies from FreeCurrency API");
        }
    }
}
