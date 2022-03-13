using CurrencyConverter.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverter
{
    public class Conversion
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 });

        public Conversion(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<IEnumerable<string>> FetchAllAvailableCurrencies(CancellationToken cancellationToken)
        {
            var currencies = await GetOrCreateCurrencyResponse(FetchCurrencies, cancellationToken);
            List<string> list = currencies.Data.Keys.ToList();
            list.Add(currencies.Query.BaseCurrency);
            list.Sort();
            return list.Distinct();
        }

        public async Task<CurrencyConversionResponse> ConvertCurrencyAsync(string currencyFrom, string currencyTo, double value, CancellationToken cancellationToken)
        {
            if (double.IsNegative(value) || !double.IsNormal(value)) throw new ArgumentException($"Value of {nameof(value)} '{value}' is not valid!");
            if (currencyFrom == null) throw new ArgumentException($"Value of {nameof(currencyFrom)} cannot be null");
            if (currencyTo == null) throw new ArgumentException($"Value of {nameof(currencyTo)} cannot be null");

            var currencies = await FetchCurrencies(cancellationToken);
            var data = currencies.Data;
            if (!data.ContainsKey(currencies.Query.BaseCurrency)) data.Add(currencies.Query.BaseCurrency, 1f);
            if (!data.ContainsKey(currencyFrom)) throw new KeyNotFoundException($"The parameter {nameof(currencyFrom)}, {currencyFrom}, is not a valid currency");
            if (!data.ContainsKey(currencyTo)) throw new KeyNotFoundException($"The parameter {nameof(currencyTo)}, {currencyTo}, is not a valid currency");

            return new CurrencyConversionResponse
            {
                ConvertedAmount = (data[currencyTo] / data[currencyFrom]) * value,
                CurrencyConverteredOrigin = currencyFrom,
                CurrencyConverteredTarget = currencyTo,
                OriginalAmount = value
            };
        }

        private async Task<CurrencyResponse> GetOrCreateCurrencyResponse(Func<CancellationToken, Task<CurrencyResponse>> fetchCurrencies, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue("key", out CurrencyResponse cacheEntry))// Look for cache key.
            {
                cacheEntry = await fetchCurrencies(cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    Size = 1,
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(TimeSpan.FromMinutes(1).TotalSeconds),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(TimeSpan.FromMinutes(10).TotalSeconds),
                };
                _cache.Set("key", cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        private async Task<CurrencyResponse> FetchCurrencies(CancellationToken cancellationToken)
        {
            // Consider a caching solution to reduce amount of calls to freecurrency api
            var request = await _client.GetAsync(_configuration.GetConnectionString("freeCurrencyApi"), cancellationToken);
            if (request.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStringAsync();
                var currencies = JsonConvert.DeserializeObject<CurrencyResponse>(content);
                return currencies;
            }
            throw new Exception($"Failed to fetch currencies: {request.StatusCode}");
        }
    }
}
