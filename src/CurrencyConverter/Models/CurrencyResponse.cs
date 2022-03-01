using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurrencyConverter
{
    public class CurrencyResponse
    {
        public Query Query { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string, double> Data { get; set; }
    }

    public class Query
    {
        public string ApiKey { get; set; }
        public int Timestamp { get; set; }
        [JsonProperty("base_currency")]
        public string BaseCurrency { get; set; }
    }
}
