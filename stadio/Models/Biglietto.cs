using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stadio.Models
{
    public class Biglietto
    {
        public int CodPartita { get; set; }

        public string nomePartita { get; set; }

        public string dataPartita { get; set; }

        public int possessore { get; set; }

        public int Pagante { get; set; }

        public string settore { get; set; }

        public int posto { get; set; }

        public string codice { get; set; }

        public decimal importo { get; set; }

        public int PostiLib { get; set; }

        public decimal ContoUtente { get; set; }

        public string SetAbb { get; set; }

        public string CompPartita { get; set; }

        public string Ospiti { get; set; }
    }
}