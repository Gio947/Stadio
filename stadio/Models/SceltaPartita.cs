using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace stadio.Models
{
    public class SceltaPartita
    {
        [Required]
        public int Tessera { get; set; }

        [Required]
        public string competizione { get; set; }

        [Required]
        public string squadra { get; set; }

        public string message { get; set; }
    }
    public class SceltaPrezzo: SceltaPartita
    {
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal prezzo { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal sconto { get; set; }
    }
    public class SceltaSettore : SceltaPartita
    {
        [Required]
        public string settore { get; set; }
    }
    public class SceltaPosto : SceltaSettore
    {
        [Required]
        public int posto { get; set; }
    }
}