using stadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace stadio.Controllers
{
    public class AdminController : Controller
    {
        public static int idUtente = 0;
        public ActionResult Index()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult SezionePersonale()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Partita()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Partita p = new Partita();

                return View("Partita", p);
            }
        }

        [HttpPost]
        public ActionResult Partita(Partita p)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (p.dataInizio <= DateTime.Now)
                {
                    p.message = "la data indicata non è valida. ";
                    return View("Partita", p);
                }
                if (ModelState.IsValid)
                {
                    var wcf = new ServiceReference1.Service1Client();
                    
                    //funzione che mi mi conferma o smentisce il successo del caricamento dei dati riferibili ad un determinato evento.
                    var aggiungiPartita = wcf.AggiungiPartita(new ServiceReference1.Partita()
                    {
                        squadra1 = p.squadra1,
                        squadra2 = p.squadra2,
                        competizione = p.competizione,
                        dataInizio = p.dataInizio,
                        orainizio = p.orainizio,
                        importoCurvaLocali = p.importoCurvaLocali,
                        importoCurvaOspiti = p.importoCurvaOspiti,
                        importoDistinti = p.importoDistinti,
                        importoLaterali = p.importoLaterali
                    });

                    if (aggiungiPartita)
                    {
                        return View("Index");
                    }
                    else
                    {
                        return View("Partita");
                    }

                }
                else
                {
                    return View("Partita");
                }
            }
        }

        public ActionResult Statistica()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var wcf = new ServiceReference1.Service1Client();

                Dictionary<string, int> biglietti = new Dictionary<string, int>();
                List<Dictionary<string, int>> categorieUtenti = new List<Dictionary<string, int>>();

                var lista = wcf.Statistiche();

                foreach (var l in lista)
                {
                    biglietti.Add(l.Key, l.Value);
                }

                Statistiche s = new Statistiche();
                s.setnumBiglietti(biglietti);

                //prendo tutte le partite
                var listaPartite = wcf.PartiteTotali();

                foreach (var partita in listaPartite)
                {
                    Partita p = new Partita();

                    p.id = partita.id;
                    //passo ogni partita a StatisticheEta
                    var listaEta = wcf.StatisticheEta(p.id);

                    foreach (var l in listaEta)
                    {
                        //ogni coppia eta-biglietti la assegno al nuovo dizionario
                        Dictionary<string, int> cat = new Dictionary<string, int>();
                        cat.Add(l.Key, l.Value);
                        categorieUtenti.Add(cat);
                    }
                    s.setListaEta(categorieUtenti);
                }
          
                return View("Statistica", s);
            }
        }
        public ActionResult Sconto()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Sconto s = new Sconto();

                return View("Sconto", s);
            }
        }

        [HttpPost]
        public ActionResult Sconto(Sconto s)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (s.inizio < DateTime.Now)
                {
                    s.message1 = "la data di inizio validità non può essere passata. ";
                    return View("Sconto", s);
                }
                if (s.fine < s.inizio)
                {
                    s.message2 = "la data di fine validità non può essere antecedente alla data d'inizio. ";
                    return View("Sconto", s);
                }
                if (((s.giovane > 0 && s.scontoUnder > 0 && s.scontoUnder < 101) || (s.anziano > 0 && s.scontoOver > 0 && s.scontoOver < 101) || s.scontoDonna > 0) && (s.inizio > DateTime.Now && s.fine > s.inizio))
                {
                    var wcf = new ServiceReference1.Service1Client();

                    //aggiorno i dati riferibili alla tabella sconti.
                    var aggiorna = wcf.ModificaSconti(new ServiceReference1.Sconto()
                    {
                        giovane = s.giovane,
                        scontoUnder = s.scontoUnder,
                        anziano = s.anziano,
                        scontoOver = s.scontoOver,
                        scontoDonna = s.scontoDonna,
                        inizio = s.inizio,
                        fine = s.fine

                    });
                    if (aggiorna)
                    {
                        return View("Index");
                    }
                    else
                    {
                        if (ModelState.IsValid)
                            return View("Sconto");
                        else
                            return View("Sconto", s);
                    }
                }

                if (!ModelState.IsValid)
                    return View("Sconto");
                else
                    return View("Sconto", s);
            }
        }
    }
}