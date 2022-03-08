using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public async Task<CurrencyConversionResponse> ConvertCurrencyAsync(Currency convertion)
        {
            var currencies = await FetchCurrencies();
            var data = currencies.Data;
            if (!data.ContainsKey(currencies.Query.BaseCurrency)) data.Add(currencies.Query.BaseCurrency, 1f); 
            if (!data.ContainsKey(convertion.CurrencyFrom)) throw new KeyNotFoundException($"The parameter {nameof(convertion.CurrencyFrom)}, {convertion.CurrencyFrom}, is not a valid currency");
            if (!data.ContainsKey(convertion.CurrencyTo)) throw new KeyNotFoundException($"The parameter {nameof(convertion.CurrencyTo)}, {convertion.CurrencyTo}, is not a valid currency");

            return new CurrencyConversionResponse
            {
                ConvertedAmount = (data[convertion.CurrencyTo] / data[convertion.CurrencyFrom]) * convertion.Amount,
                CurrencyConverteredOrigin = convertion.CurrencyFrom,
                CurrencyConverteredTarget = convertion.CurrencyTo,
                OriginalAmount = convertion.Amount
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
