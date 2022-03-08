using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class Currency
    {
        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "The field {0} must be a positive, non-zero number.")]
        public double Amount { get; set; }

        [NotNull]
        [Required]
        public string CurrencyFrom { get; set; }

        [NotNull]
        [Required]
        public string CurrencyTo { get; set; }
    }
}
