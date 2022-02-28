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

        public async Task<double> ConvertCurrencyAsync(string currencyFrom, string currencyTo, double value)
        {
            var currencies = await FetchCurrencies();
            var data = currencies.Data;
            if (data.ContainsKey(currencyFrom) && data.ContainsKey(currencyTo))
            {
                return (data[currencyTo] / data[currencyFrom]) * value;
            }
            throw new ArgumentException("bad request");
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
            throw new Exception("something went wrong!");
        }
    }
}
