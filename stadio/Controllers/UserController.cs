using stadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace stadio.Controllers
{
    public class UserController : Controller
    {
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

        public ActionResult Conto()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login","Home");
            }
            else
            {
                Utente u = new Utente() 
                { 
                    id = Convert.ToInt32(Session["Id"])
                };

                return View("Conto", u);
            }
        }

        public ActionResult SceltaPartita()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SceltaPartita sp = new SceltaPartita();

                return View("SceltaPartita", sp);
            }
        }

        public ActionResult SceltaSettore()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SceltaSettore sceltasettore = new SceltaSettore();

                return View("SceltaSettore", sceltasettore);
            }
        }

        public ActionResult SceltaPosto()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SceltaPosto sceltaposto = new SceltaPosto();

                return View("SceltaPosto", sceltaposto);
            }
        }

        public ActionResult SceltaPrezzo()
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SceltaPrezzo sceltaprezzo = new SceltaPrezzo();

                return View("SceltaPrezzo", sceltaprezzo);
            }
        }

        [HttpPost]
        public ActionResult SceltaPartita(SceltaPartita sp)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var wcf = new ServiceReference1.Service1Client();

                    //tramite i dati indicati dall'utente cerco un posto da assegnargli.
                    var trovato = wcf.TrovaPostoPartita(new ServiceReference1.SceltaPartita()
                    {
                        competizione = sp.competizione,
                        squadra = sp.squadra,
                        Tessera = sp.Tessera
                    });

                    if (trovato.possessore == 0)
                    {
                        sp.message = "Posto già prenotato per questo evento. ";
                        return View("SceltaPartita", sp);
                    }
                    if (trovato.PostiLib == 0)
                    {
                        sp.message = "Posti esauriti per questo evento. ";
                        return View("SceltaPartita", sp);
                    }

                    //controllo se l'utente ha diritto a sconti.
                    var scontato = wcf.ApplicaSconto(new ServiceReference1.Biglietto()
                    {
                        possessore = trovato.possessore,
                        importo = trovato.importo
                    });

                    trovato.importo = scontato;

                    //procedo alla transazione per l'acquisto del biglietto.
                    var finale = wcf.NuovoBiglietto(new ServiceReference1.Biglietto()
                    {
                        CodPartita = trovato.CodPartita,
                        possessore = trovato.possessore,
                        Pagante = Convert.ToInt32(Session["Id"].ToString()),
                        settore = trovato.settore,
                        posto = trovato.posto,
                        importo = trovato.importo,
                        ContoUtente = Convert.ToDecimal(Session["Conto"].ToString()),
                        SetAbb = trovato.SetAbb,
                        PostiLib = trovato.PostiLib,
                        dataPartita = trovato.dataPartita,
                        Ospiti = trovato.Ospiti,
                        CompPartita = trovato.CompPartita
                    });

                    if (finale.message == " ")
                    {
                        //aggiorno il conto dell'utente che ha provveduto all'acquisto del biglietto.
                        var aggiorna = wcf.AggiornaConto(new ServiceReference1.Utente()
                        {
                            id = Convert.ToInt32(Session["Id"].ToString()),
                            conto = 0
                        });
                        Session["Conto"] = aggiorna.conto;
                        return RedirectToAction("SezionePersonale");
                    }
                    else if (finale.message == "biglietto già prenotato. ")
                    {
                        return RedirectToAction("SceltaPartita", sp);
                    }
                    else
                    {
                        sp.message = finale.message;
                        return View("SceltaPartita", sp);
                    }
                }
                else
                {
                    return View("SceltaPartita", sp);
                }
            }
        }

        [HttpPost]
        public ActionResult SceltaSettore(SceltaSettore sceltasettore)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var wcf = new ServiceReference1.Service1Client();

                    //tramite i dati indicati dall'utente cerco un posto da assegnargli.
                    var trovato = wcf.TrovaPostoSettore(new ServiceReference1.SceltaSettore()
                    {
                        competizione = sceltasettore.competizione,
                        squadra = sceltasettore.squadra,
                        settore = sceltasettore.settore,
                        Tessera = sceltasettore.Tessera
                    });

                    if (trovato.possessore == 0)
                    {
                        sceltasettore.message = "Posto già prenotato per questo evento. ";
                        return View("SceltaPartita", sceltasettore);
                    }
                    if (trovato.PostiLib == 0)
                    {
                        sceltasettore.message = "Posti esauriti per questo settore. ";
                        return View("SceltaSettore", sceltasettore);
                    }

                    //controllo se l'utente ha diritto a sconti.
                    var scontato = wcf.ApplicaSconto(new ServiceReference1.Biglietto()
                    {
                        possessore = trovato.possessore,
                        importo = trovato.importo
                    });

                    trovato.importo = scontato;

                    //procedo alla transazione per l'acquisto del biglietto.
                    var finale = wcf.NuovoBiglietto(new ServiceReference1.Biglietto()
                    {
                        CodPartita = trovato.CodPartita,
                        possessore = trovato.possessore,
                        Pagante = Convert.ToInt32(Session["Id"].ToString()),
                        settore = trovato.settore,
                        posto = trovato.posto,
                        importo = trovato.importo,
                        ContoUtente = Convert.ToDecimal(Session["Conto"].ToString()),
                        SetAbb = trovato.SetAbb,
                        PostiLib = trovato.PostiLib,
                        dataPartita = trovato.dataPartita,
                        Ospiti = trovato.Ospiti,
                        CompPartita = trovato.CompPartita
                    });

                    if (finale.message == " ")
                    {
                        //aggiorno il conto dell'utente che ha provveduto all'acquisto del biglietto.
                        var aggiorna = wcf.AggiornaConto(new ServiceReference1.Utente()
                        {
                            id = Convert.ToInt32(Session["Id"].ToString()),
                            conto = 0
                        });
                        Session["Conto"] = aggiorna.conto;
                        return RedirectToAction("SezionePersonale");
                    }
                    else if (finale.message == "biglietto già prenotato. ")
                    {
                        return RedirectToAction("SceltaSettore", sceltasettore);
                    }
                    else
                    {
                        sceltasettore.message = finale.message;
                        return View("SceltaSettore", sceltasettore);
                    }
                }
                else
                {
                    return View("SceltaSettore", sceltasettore);
                }
            }
        }

        [HttpPost]
        public ActionResult SceltaPosto(SceltaPosto sceltaposto)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var wcf = new ServiceReference1.Service1Client();

                    //tramite i dati indicati dall'utente controllo se posso assegnare il posto all'utente.
                    var trovato = wcf.TrovaPostoPosto(new ServiceReference1.SceltaPosto()
                    {
                        competizione = sceltaposto.competizione,
                        squadra = sceltaposto.squadra,
                        settore = sceltaposto.settore,
                        posto = sceltaposto.posto,
                        Tessera = sceltaposto.Tessera
                    });

                    if (trovato.PostiLib == 0)
                    {
                        sceltaposto.message = "Posti esauriti per questo evento. ";
                        return View("SceltaPosto", sceltaposto);
                    }
                    if (trovato.posto == 0)
                    {
                        sceltaposto.message = "Posto già prenotato. ";
                        return View("SceltaPosto", sceltaposto);
                    }

                    //controllo se l'utente ha diritto a sconti.
                    var scontato = wcf.ApplicaSconto(new ServiceReference1.Biglietto()
                    {
                        possessore = trovato.possessore,
                        importo = trovato.importo
                    });

                    trovato.importo = scontato;

                    //procedo alla transazione per l'acquisto del biglietto.
                    var finale = wcf.NuovoBiglietto(new ServiceReference1.Biglietto()
                    {
                        CodPartita = trovato.CodPartita,
                        possessore = trovato.possessore,
                        Pagante = Convert.ToInt32(Session["Id"].ToString()),
                        settore = trovato.settore,
                        posto = trovato.posto,
                        importo = trovato.importo,
                        ContoUtente = Convert.ToDecimal(Session["Conto"].ToString()),
                        SetAbb = trovato.SetAbb,
                        PostiLib = trovato.PostiLib,
                        dataPartita = trovato.dataPartita,
                        Ospiti = trovato.Ospiti,
                        CompPartita = trovato.CompPartita
                    });

                    if (finale.message == " ")
                    {
                        //aggiorno il conto dell'utente che ha provveduto all'acquisto del biglietto.
                        var aggiorna = wcf.AggiornaConto(new ServiceReference1.Utente()
                        {
                            id = Convert.ToInt32(Session["Id"].ToString()),
                            conto = 0
                        });
                        Session["Conto"] = aggiorna.conto;
                        return RedirectToAction("SezionePersonale");
                    }

                    else
                    {
                        sceltaposto.message = finale.message;
                        return View("SceltaPosto", sceltaposto);
                    }
                }
                else
                {
                    return View("SceltaPosto", sceltaposto);
                }
            }
        }

        [HttpPost]
        public ActionResult SceltaPrezzo(SceltaPrezzo sceltaprezzo)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var wcf = new ServiceReference1.Service1Client();

                    //controllo se l'utente ha diritto a sconti.
                    var scontato = wcf.ApplicaSconto(new ServiceReference1.Biglietto()
                    {
                        possessore = sceltaprezzo.Tessera,
                        importo = 1
                    });

                    //tramite i dati indicati dall'utente e lo sconto di cui beneficia cerco un posto disponibile.
                    var trovato = wcf.TrovaPostoPrezzo(new ServiceReference1.SceltaPrezzo()
                    {
                        competizione = sceltaprezzo.competizione,
                        squadra = sceltaprezzo.squadra,
                        prezzo = sceltaprezzo.prezzo,
                        sconto = scontato,
                        Tessera = sceltaprezzo.Tessera,
                    });

                    if (trovato.PostiLib == 0)
                    {
                        sceltaprezzo.message = "Non ci sono posti disponibili a questo prezzo. ";
                        return View("SceltaPrezzo", sceltaprezzo);
                    }

                    //procedo alla transazione per l'acquisto del biglietto.
                    var finale = wcf.NuovoBiglietto(new ServiceReference1.Biglietto()
                    {
                        CodPartita = trovato.CodPartita,
                        possessore = trovato.possessore,
                        Pagante = Convert.ToInt32(Session["Id"].ToString()),
                        settore = trovato.settore,
                        posto = trovato.posto,
                        importo = trovato.importo,
                        ContoUtente = Convert.ToDecimal(Session["Conto"].ToString()),
                        SetAbb = trovato.SetAbb,
                        PostiLib = trovato.PostiLib,
                        dataPartita = trovato.dataPartita,
                        Ospiti = trovato.Ospiti,
                        CompPartita = trovato.CompPartita
                    });

                    if (finale.message == " ")
                    {
                        //aggiorno il conto dell'utente che ha provveduto all'acquisto del biglietto.
                        var aggiorna = wcf.AggiornaConto(new ServiceReference1.Utente()
                        {
                            id = Convert.ToInt32(Session["Id"].ToString()),
                            conto = 0
                        });
                        Session["Conto"] = aggiorna.conto;
                        return RedirectToAction("SezionePersonale");
                    }
                    else if (finale.message == "biglietto già prenotato. ")
                    {
                        return RedirectToAction("SceltaPrezzo", sceltaprezzo);
                    }
                    else
                    {
                        sceltaprezzo.message = finale.message;
                        return View("SceltaPrezzo", sceltaprezzo);
                    }
                }
                else
                {
                    return View("SceltaPrezzo", sceltaprezzo);
                }
            }
        }

        [HttpPost]
        public ActionResult Conto(Utente u)
        {
            if (Session["Id"] is null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (u.conto > 0)
                {
                    var wcf = new ServiceReference1.Service1Client();

                    //aggiorno il conto dell'utente che ha provveduto alla ricarica del conto.
                    var aggiorna = wcf.AggiornaConto(new ServiceReference1.Utente()
                    {
                        id = Convert.ToInt32(Session["Id"]),
                        conto = u.conto

                    });

                    if (aggiorna.conto == 0)
                    {
                        return View("Conto", u);
                    }
                    else
                    {
                        Session["Conto"] = aggiorna.conto;
                        return RedirectToAction("SezionePersonale");
                    }
                }
                else if (!ModelState.IsValid)
                    return View("Conto", u);
                else
                    return View("Conto", u);
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
                var wcf = new ServiceReference1.Service1Client();

                //cerco la lista dei biglietti prenotati dall'utente.
                var lista = wcf.BigliettiPassati(Convert.ToInt32(Session["Id"]));
                if (lista.id > 0)
                {
                    return View("SezionePersonale", lista);
                }
                else
                {
                    return View("Index");
                }
            }             
        }
    }
}