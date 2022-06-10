using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace stadio.Models
{
    public class Sconto
    {
        [Display(Name = "età Under")]
        [Required]
        [Range(1, Double.PositiveInfinity, ErrorMessage = "il valore relativo all'età non può essere inferiore a {1}. ")]
        public int giovane { get; set; }

        [Display(Name = "sconto Under")]
        [Required]
        [Range(1, 100, ErrorMessage = "il valore di sconto può essere compreso fra 0 e {2}. ")]
        public int scontoUnder { get; set; }

        [Display(Name = "sconto Donna")]
        [Required]
        [Range(1, 100, ErrorMessage = "il valore di sconto può essere compreso fra 0 e {2}. ")]
        public int scontoDonna { get; set; }

        [Display(Name = "età Over")]
        [Required]
        [Range(1, Double.PositiveInfinity, ErrorMessage = "il valore relativo all'età non può essere inferiore a {1}. ")]
        public int anziano { get; set; }

        [Display(Name = "sconto Over")]
        [Required]
        [Range(1, 100, ErrorMessage = "il valore di sconto può essere compreso fra 0 e {2}. ")]
        public int scontoOver { get; set; }

        [Required]
        [Display(Name = "inizio validità")]
        [DataType(DataType.Date)]
        public DateTime inizio { get; set; }

        public string message1 { get; set; }

        [Required]
        [Display(Name = "fine validità")]
        [DataType(DataType.Date)]
        public DateTime fine { get; set; }

        public string message2 { get; set; }
    }
}