using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace stadio.Models
{
    public class Settore
    {
        [Required]
        [StringLength(30)]
        public string nome { get; set; }

        [Required]
        [StringLength(5)]
        public int num_posti { get; set; }

        [Required]
        [StringLength(15)]
        public string tipo { get; set; }

        [Required]
        [StringLength(3)]
        public string abbreviazione { get; set; }

        public List<Posto> Posti = new List<Posto>();

        public void Riempi()
        {
            for (int i = 0; i < this.num_posti; i++)
            {
                Posto posto = new Posto();
                Posto p = posto;
                p.Numero = i + 1;
                p.Libero = true;
                p.Importo = 0;
            }
        }
    }

}