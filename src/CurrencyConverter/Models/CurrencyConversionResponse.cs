using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class CurrencyConversionResponse
    {
        public string CurrencyConverteredOrigin { get; set; }
        public string CurrencyConverteredTarget { get; set; }
        public double OriginalAmount { get; set; }
        public double ConvertedAmount { get; set; }
    }
}
