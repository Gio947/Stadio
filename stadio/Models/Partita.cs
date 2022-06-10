using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace stadio.Models
{
    public class Partita
    {
        public int id { get; set; }

        [Required]
        [StringLength(15)]
        public string competizione { get; set; }

        [Required]
        [StringLength(50)]
        public string squadra1 { get; set; }

        [Required]
        [StringLength(50)]
        public string squadra2 { get; set; }

        [StringLength(60)]
        public string nome { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime dataInizio { get; set; }

        public string message { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime orainizio { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal importoCurvaLocali { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal importoCurvaOspiti { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal importoDistinti { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal importoLaterali { get; set; }

        public List<Posto> CurvaNord = new List<Posto>();

        public List<Posto> CurvaSud = new List<Posto>();

        public List<Posto> DistintiOvest1 = new List<Posto>();

        public List<Posto> DistintiOvest2 = new List<Posto>();

        public List<Posto> DistintiEst1 = new List<Posto>();

        public List<Posto> DistintiEst2 = new List<Posto>();

        public List<Posto> Laterali1 = new List<Posto>();

        public List<Posto> Laterali2 = new List<Posto>();

        public List<Posto> Laterali3 = new List<Posto>();

        public List<Posto> Laterali4 = new List<Posto>();
    }
}