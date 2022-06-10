using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stadio.Models
{
    public class Statistiche
    {
        public Dictionary<string, int> numBiglietti { get; set; }

        public List<Dictionary<string, int>> etaUtenti { get; set; }

        public void setnumBiglietti(Dictionary<string, int> lista)
        {
            numBiglietti = lista;
        }

        public void setListaEta(List<Dictionary<string, int>> lista)
        {
            etaUtenti = lista;
        }

        public void stampaSt(int i)
        {
            foreach (var el in etaUtenti[i])
            {
                Console.WriteLine(el.Key + " " + el.Value);
            }
        }

    }
}