using System;
using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Models
{
    public class Currency
    {
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Amount { get; set; }

        [Required]
        public string CurrencyFrom { get; set; }

        [Required]
        public string CurrencyTo { get; set; }
    }
}
