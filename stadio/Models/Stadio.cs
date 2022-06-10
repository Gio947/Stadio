using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace stadio.Models
{
    public class Stadio
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string nome { get; set; }

        [Required]
        [StringLength(50)]
        public string indirizzo { get; set; }

        [Required]
        [StringLength(25)]
        public string citta { get; set; }

        public List<Settore> Settori = new List<Settore>();

    }
}