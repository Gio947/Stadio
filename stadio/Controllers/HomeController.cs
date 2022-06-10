using Microsoft.AspNetCore.Mvc;
using stadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace stadio.Controllers
{
    public class HomeController : Controller
    {
        public int idUtente;
        public ActionResult Index()
        {
            Session.Clear();
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Registrazione()
        {
            Utente utente = new Utente();

            return View("Registrazione", utente);
        }

        [HttpPost]
        public ActionResult Registrazione(Utente utente)
        {
            if (!ControlloDataReg(utente.dataNascita))
                return View("Registrazione", utente);
            if (ModelState.IsValid)
            {
                var wcf = new ServiceReference1.Service1Client();

                var registrazione = wcf.Registrazione(new ServiceReference1.Utente()
                {
                    username = utente.username,
                    password = utente.password,
                    dataNascita = utente.dataNascita.Date,
                    sesso = utente.sesso,
                    conto = 0,
                    livello = "User",
                    nome = utente.nome,
                    cognome = utente.cognome
                });

                if (registrazione.id > 0)
                {
                    Session["Nome"] = registrazione.nome;
                    Session["Cognome"] = registrazione.cognome;
                    Session["Id"] = registrazione.id;
                    Session["Sesso"] = registrazione.sesso;
                    Session["Nascita"] = registrazione.dataNascita.ToString().Substring(0,10);
                    Session["Conto"] = registrazione.conto;
                    Session["Livello"] = registrazione.livello;

                    idUtente = registrazione.id;

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ViewBag.Message = "Errore nella registrazione";
                    return View("Registrazione");
                }
            }
            else
            {
                return View("Registrazione", utente);
            }
        }

        [HttpPost]
        public JsonResult IsAlreadySigned(string username)
        {

            return Json(IsUserAvailable(username));

        }
        public bool IsUserAvailable(string User)
        {
            Utente utente = new Utente()
            {
                username = User
            };

            var wcf = new ServiceReference1.Service1Client();

            //controllo se l'username è già presente nel database.
            var status = wcf.IsUserAvailable(new ServiceReference1.Utente()
        {
            username = utente.username
        });
            return status;
        }

        [HttpPost]
        public JsonResult IsDataRegFuture(DateTime datanascita)
        {

            return Json(ControlloDataReg(datanascita));

        }
        public bool ControlloDataReg(DateTime nascita)
        {
            //controllo, durante la registrazione, che la data di nascita dell'utente non sia maggiore della data odierna.
            if (nascita > DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ActionResult Login()
        {
            Utente utente = new Utente();

            return View("Login", utente);
        }

        [HttpPost]
        public ActionResult Login(Account account)
        {
            if (ModelState.IsValid)
            {
                var wcf = new ServiceReference1.Service1Client();

                var login = wcf.Login(new ServiceReference1.Account()
                {
                    username = account.username,
                    password = account.password
                });

                Session["Nome"] = login.nome;
                Session["Cognome"] = login.cognome;
                Session["Id"] = login.id;
                Session["Sesso"] = login.sesso;
                Session["Nascita"] = login.dataNascita.ToString().Substring(0, 10);
                Session["Conto"] = login.conto;
                Session["Livello"] = login.livello;

                if (login.livello == "User")
                {
                    idUtente = login.id;

                    return RedirectToAction("Index", "User");
                }
                else if (login.livello == "Admin")
                {
                    idUtente = login.id;

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    account.accesso = "Errore nel username e/o nella password";
                    return View("Login", account);
                }
            }
            else
            {
                return View("Login",account);
            }
        }
    }
}