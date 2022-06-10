using MySql.Data.MySqlClient;
using stadio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFStadio
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "Service1" nel codice e nel file di configurazione contemporaneamente.
    public class Service1 : IService1
    {
        public Utente Login(Account account)
        {
            try
            {
                Utente utente = new Utente();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo se ci sono account registrati a database con l'username inserito.
                    comando.CommandText = "SELECT * FROM utente where Username= '" + account.username + "'";
                    var result = comando.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            if (result["Password"].ToString() == account.password)
                            {
                                // se username e password sono corretti creo un oggetto utente con tutti i dati che scarico dal database convertiti in modo opportuno.
                                utente.username = result["Username"].ToString();
                                utente.password = result["Password"].ToString();
                                utente.nome = result["Nome"].ToString();
                                utente.cognome = result["Cognome"].ToString();
                                utente.sesso = result["Sesso"].ToString();
                                utente.dataNascita = Convert.ToDateTime(result["DataNascita"].ToString());
                                utente.conto = Convert.ToDecimal(result["Conto"].ToString());
                                utente.livello = result["Livello"].ToString();
                                utente.id = Convert.ToInt32(result["Id"].ToString());
                                Console.WriteLine("Accesso eseguito.");
                                return utente;
                            }
                            else
                            {
                                //se l'abbinamento username-password è errato restituisco un utente di default con tutti i valori uguali a zero.
                                utente.username = "0";
                                utente.password = "0";
                                utente.nome = "0";
                                utente.cognome = "0";
                                utente.sesso = "0";
                                utente.dataNascita = DateTime.Now;
                                utente.conto = 0;
                                utente.livello = "0";
                                utente.id = 0;
                                Console.WriteLine("Password errata.");
                                return utente;
                            }
                        }
                    }

                    else
                    {
                        //se non esistono account registrati con l'username indicato restituisco un account di default con tutti i valori uguali a zero.
                        utente.username = "0";
                        utente.password = "0";
                        utente.nome = "0";
                        utente.cognome = "0";
                        utente.sesso = "0";
                        utente.dataNascita = DateTime.Now;
                        utente.conto = 0;
                        utente.livello = "0";
                        utente.id = 0;
                        Console.WriteLine("Errore durante il login.");
                        return utente;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public Utente Registrazione(Utente utente)
        {
            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo se ci sono account registrati a database con l'username inserito.
                    comando.CommandText = "SELECT * FROM utente WHERE username = '" + utente.username + "' ";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        Console.WriteLine("Esiste già un utente con questo username.");
                        return utente;
                    }
                    else
                    {
                        conn.Close();
                        conn.Open();
                        using (MySqlCommand comandoInsert = conn.CreateCommand())
                        {
                            //aggiungo il nuovo utente.
                            comandoInsert.CommandText = "INSERT INTO utente VALUES('', '" + utente.nome + "', '" + utente.cognome + "', '" + utente.sesso + "', '" + utente.dataNascita.ToString("yyyy-MM-dd") + "', '" + utente.username + "' , '" + utente.password + "' , 'User' , '0')";
                            var resultInsert = comandoInsert.ExecuteNonQuery();
                            if (resultInsert > 0)
                            {
                                //controllo i valori che vengono inseriti da sistema e li aggiungo al mio oggetto utente.
                                comando.CommandText = "SELECT * FROM utente WHERE username = '" + utente.username + "'";
                                var resultut = comando.ExecuteReader();
                                while (resultut.Read())
                                {
                                    utente.livello = resultut["Livello"].ToString();
                                    utente.conto = Convert.ToDecimal(resultut["Conto"].ToString());
                                    utente.id = Convert.ToInt32(resultut["Id"]);
                                }
                                Console.WriteLine("Registrazione ok");
                                return utente;
                            }
                            if (resultInsert == 0)
                            {
                                Console.WriteLine("Errore durante la registrazione");
                                utente.id = 0;
                                return utente;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //se la registrazione ha problemi per cui non riesce a procedere dopo aver controllato anche le eccezioni applico un valore id che mi da sempre errore.
            utente.id = 0;
            return utente;
        }

        public bool AggiungiPartita(Partita p)
        {
            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                string nome = p.competizione + ": " + p.squadra1 + " - " + p.squadra2;

                using (MySqlCommand comandoSelect = conn.CreateCommand())
                {
                    //controllo se la partita che voglio inserire non sia già stata registrata.
                    comandoSelect.CommandText = "SELECT * FROM `partita` " +
                        "WHERE `nome`= '" + nome + "' ";
                    var resultSelect = comandoSelect.ExecuteReader();
                    if (resultSelect.HasRows)
                    {
                        Console.WriteLine("E' già presente una partita registrata con questo nome. ");
                        conn.Close();
                        return false;
                    }
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comandoInsert = conn.CreateCommand())
                {
                    //inserisco a database un nuovo record di una partita
                    comandoInsert.CommandText = "INSERT INTO partita VALUES('', '" + nome + "', '" + p.dataInizio.ToString("yyyy-MM-dd") + "', '" + p.orainizio.ToString("HH:mm:ss") + "', '" + p.squadra1 + "', '" + p.squadra2 + "' , '" + p.competizione + "')";
                    var resultInsert = comandoInsert.ExecuteNonQuery();
                    if (resultInsert > 0)
                    {
                        Console.WriteLine("registrazione partita con successo");
                    }
                    if (resultInsert == 0)
                    {
                        Console.WriteLine("Errore durante la registrazione della partita");
                        return false;
                    }
                }
                using (MySqlCommand comandoControl = conn.CreateCommand())
                {
                    //raccolgo i dati dei settori e l'id partita per andare a creare i record in composizione.
                    comandoControl.CommandText =
                        "SELECT Tipo, settore.Nome, NPosti,IDPartita " +
                        "FROM settore JOIN partita " +
                        "WHERE `IdStadio` = 1 " +
                        "AND `Competizione` = '" + p.competizione + "' " +
                        "AND `SquadraIN` = '" + p.squadra1 + "' " +
                        "AND `SquadraOUT` = '" + p.squadra2 + "' ";

                    var resultControl = comandoControl.ExecuteReader();
                    if (resultControl.HasRows)
                    {
                        int i = 0;
                        string query = "INSERT INTO composizione (`CodPartita`, `Settore`, `PostiLib`, `Importo`) VALUES";
                        while (resultControl.Read())
                        {
                            string settore = resultControl["Nome"].ToString();
                            string tipo = resultControl["Tipo"].ToString();
                            //controllo se è la prima tupla di valori che inserisco.
                            if (i > 0)
                            {
                                query = query + ",";
                            }
                            else
                            {
                                i++;
                            }
                            //compongo le tuple di valori da inserire in database nella tabella composizione in base al tipo o al nome del settore.
                            if (settore == "CurvaNord")
                            {
                                query = query + "(" + Convert.ToInt32(resultControl["IDPartita"]) + ", '" + resultControl["Nome"].ToString() + "' ," + Convert.ToInt32(resultControl["NPosti"]) + "," + p.importoCurvaLocali + ")";
                            }
                            else if (settore == "CurvaSud")
                            {
                                query = query + "(" + Convert.ToInt32(resultControl["IDPartita"]) + ", '" + resultControl["Nome"].ToString() + "' ," + Convert.ToInt32(resultControl["NPosti"]) + "," + p.importoCurvaOspiti + ")";
                            }
                            else if (tipo == "Distinti")
                            {
                                query = query + "(" + Convert.ToInt32(resultControl["IDPartita"]) + ", '" + resultControl["Nome"].ToString() + "' ," + Convert.ToInt32(resultControl["NPosti"]) + "," + p.importoDistinti + ")";
                            }
                            else
                            {
                                query = query + "(" + Convert.ToInt32(resultControl["IDPartita"]) + ", '" + resultControl["Nome"].ToString() + "' ," + Convert.ToInt32(resultControl["NPosti"]) + "," + p.importoLaterali + ")";
                            }
                        }
                        conn.Close();
                        conn.Open();
                        comandoControl.CommandText = query;

                        // inserisco le tuple appena create a database.
                        var resultSector = comandoControl.ExecuteNonQuery();
                        if (resultSector > 0)
                        {
                            Console.WriteLine("registrazione composizione stadio con successo. ");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("registrazione composizione stadio fallita. ");
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;

        }

        public Utente AggiornaConto(Utente u)
        {
            try
            {
                Utente utente = new Utente();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //ottengo il saldo attuale dell'utente.
                    comando.CommandText = "SELECT Conto FROM utente Where utente.Id = '" + u.id + "'";
                    var result = comando.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            utente.conto = Convert.ToDecimal(result["Conto"].ToString());
                        }
                    }
                    //aggiungo i soldi indicati dall'utente.
                    string conto = (utente.conto + u.conto).ToString().Replace(",", ".");
                    utente.conto = Convert.ToDecimal(conto.Replace(".", ","));

                    conn.Close();
                    conn.Open();
                    using (MySqlCommand comandoUpdate = conn.CreateCommand())
                    {
                        //carico a database i dati del nuovo saldo.
                        comandoUpdate.CommandText = "UPDATE utente set Conto = '" + conto + "' WHERE Id = '" + u.id + "'";
                        var resultUpdate = comandoUpdate.ExecuteNonQuery();

                        if (resultUpdate > 0)
                        {
                            Console.WriteLine("Aggiornamento conto ok");
                            return utente;
                        }
                        if (resultUpdate == 0)
                        {
                            Console.WriteLine("Errore durante l'aggiornamento del conto");
                            return utente;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public Biglietto TrovaPostoPartita(SceltaPartita sceltapartita)
        {
            try
            {
                Biglietto biglietto = new Biglietto();
                Biglietto trovato = new Biglietto();

                List<Biglietto> SetDisp = new List<Biglietto>();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo, tramite il numero della tessera, se è già stato prenotato un biglietto per l'evento a nome dell'utente indicato.
                    comando.CommandText =
                      "SELECT * " +
                      "FROM biglietto JOIN partita " +
                      "WHERE biglietto.Partita = partita.IDPartita " +
                      "AND partita.SquadraOUT = '" + sceltapartita.squadra + "' " +
                      "AND partita.Competizione = '" + sceltapartita.competizione + "' " +
                      "AND biglietto.Possessore = '" + sceltapartita.Tessera + "' ";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        conn.Close();
                        trovato.possessore = 0;
                        return trovato;
                    }
                    else
                        trovato.possessore = sceltapartita.Tessera;
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comando1 = conn.CreateCommand())
                {
                    //controllo nel database se ci sono settori con posti liberi per la partita indicata.
                    comando1.CommandText =
                      "SELECT settore.Nome, composizione.Importo, partita.IDPartita, partita.DataInizio, composizione.PostiLib, settore.Abbreviazione " +
                      "FROM composizione JOIN partita JOIN settore " +
                      "WHERE partita.IDPartita = composizione.CodPartita " +
                      "AND composizione.Settore = settore.Nome " +
                      "AND partita.SquadraOUT = '" + sceltapartita.squadra + "' " +
                      "AND partita.Competizione = '" + sceltapartita.competizione + "' " +
                      "AND composizione.PostiLib > 0 " +
                      "ORDER BY composizione.Importo DESC ";
                    var result = comando1.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            //aggiungo i dati trovati , tramite la ricerca nel database, ad una lista di tuple.
                            biglietto.settore = result["Nome"].ToString();
                            biglietto.importo = Convert.ToDecimal(result["Importo"].ToString());
                            biglietto.CodPartita = Convert.ToInt32(result["IDPartita"]);
                            biglietto.dataPartita = result["DataInizio"].ToString();
                            biglietto.PostiLib = Convert.ToInt32(result["PostiLib"]);
                            biglietto.SetAbb = result["Abbreviazione"].ToString();
                            biglietto.Ospiti = sceltapartita.squadra;
                            biglietto.CompPartita = sceltapartita.competizione;
                            biglietto.posto = 0;
                            SetDisp.Add(biglietto);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nessun posto disponibile. ");
                        trovato.PostiLib = 0;
                        conn.Close();
                        return trovato;
                    }
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comando2 = conn.CreateCommand())
                {
                    if (SetDisp.Count > 0)
                    {
                        //cerco un posto da assegnare all'utente, cercando fra ogni settore che ha posti disponibili.
                        foreach (var x in SetDisp)
                        {
                            int i = 1;
                            while (biglietto.posto == 0)
                            {
                                comando2.CommandText =
                                "SELECT biglietto.posto " +
                                "FROM biglietto JOIN partita " +
                                "WHERE biglietto.Partita = partita.IDPartita " +
                                "AND partita.competizione = '" + sceltapartita.competizione + "' " +
                                "AND partita.SquadraOut = '" + sceltapartita.squadra + "' " +
                                "AND biglietto.settore = '" + biglietto.settore + "' " +
                                "AND biglietto.Posto = '" + i + "' ";
                                var resultposto = comando2.ExecuteReader();
                                if (resultposto.HasRows)
                                {
                                    i++;
                                    comando2.Dispose();
                                    conn.Close();
                                    conn.Open();
                                }
                                else
                                {
                                    trovato.CodPartita = x.CodPartita;
                                    trovato.settore = x.settore;
                                    trovato.posto = i;
                                    trovato.importo = x.importo;
                                    trovato.dataPartita = x.dataPartita;
                                    trovato.PostiLib = x.PostiLib;
                                    trovato.SetAbb = x.SetAbb;
                                    trovato.Ospiti = x.Ospiti;
                                    trovato.CompPartita = x.CompPartita;
                                    SetDisp.Clear();
                                    conn.Close();
                                    return trovato;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public Biglietto TrovaPostoSettore(SceltaSettore sceltasettore)
        {
            try
            {
                Biglietto trovato = new Biglietto();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo, tramite il numero della tessera, se è già stato prenotato un biglietto per l'evento a nome dell'utente indicato.
                    comando.CommandText =
                      "SELECT * " +
                      "FROM biglietto JOIN partita " +
                      "WHERE biglietto.Partita = partita.IDPartita " +
                      "AND partita.SquadraOUT = '" + sceltasettore.squadra + "' " +
                      "AND partita.Competizione = '" + sceltasettore.competizione + "' " +
                      "AND biglietto.Possessore = '" + sceltasettore.Tessera + "' ";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        conn.Close();
                        trovato.possessore = 0;
                        return trovato;
                    }
                    else
                        trovato.possessore = sceltasettore.Tessera;
                }
                conn.Close();
                conn.Open();

                using (MySqlCommand comando1 = conn.CreateCommand())
                {
                    //controllo nel database se ci sono posti liberi nel settore scelto e per la partita indicata.
                    comando1.CommandText =
                      "SELECT composizione.Importo, partita.IDPartita, partita.DataInizio, composizione.PostiLib, settore.Abbreviazione " +
                      "FROM composizione JOIN partita JOIN settore " +
                      "WHERE partita.IDPartita = composizione.CodPartita " +
                      "AND composizione.Settore = settore.Nome " +
                      "AND partita.SquadraOUT = '" + sceltasettore.squadra + "' " +
                      "AND partita.Competizione = '" + sceltasettore.competizione + "' " +
                      "AND composizione.Settore = '" + sceltasettore.settore + "' " +
                      "AND composizione.PostiLib > 0  ";
                    var result = comando1.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            trovato.settore = sceltasettore.settore;
                            trovato.importo = Convert.ToDecimal(result["Importo"].ToString());
                            trovato.CodPartita = Convert.ToInt32(result["IDPartita"]);
                            trovato.dataPartita = result["DataInizio"].ToString();
                            trovato.PostiLib = Convert.ToInt32(result["PostiLib"]);
                            trovato.SetAbb = result["Abbreviazione"].ToString();
                            trovato.posto = 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nessun posto disponibile");
                        trovato.PostiLib = 0;
                        return trovato;
                    }
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comando2 = conn.CreateCommand())
                {
                    int i = 1;
                    while (trovato.posto == 0)
                    {
                        //cerco un posto da assegnare all'utente fra quelli disponibili.
                        comando2.CommandText =
                        "SELECT biglietto.posto " +
                        "FROM biglietto JOIN partita " +
                        "WHERE biglietto.Partita = partita.IDPartita " +
                        "AND partita.competizione = '" + sceltasettore.competizione + "' " +
                        "AND partita.SquadraOut = '" + sceltasettore.squadra + "' " +
                        "AND biglietto.settore = '" + sceltasettore.settore + "' " +
                        "AND biglietto.Posto = '" + i + "' ";
                        var resultposto = comando2.ExecuteReader();
                        if (resultposto.HasRows)
                        {
                            i++;
                            comando2.Dispose();
                            conn.Close();
                            conn.Open();
                        }
                        else
                        {
                            trovato.posto = i;
                            trovato.Ospiti = sceltasettore.squadra;
                            trovato.CompPartita = sceltasettore.competizione;
                            conn.Close();
                            conn.Open();
                            return trovato;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public Biglietto TrovaPostoPosto(SceltaPosto sceltaposto)
        {
            try
            {
                Biglietto trovato = new Biglietto();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo, tramite il numero della tessera, se è già stato prenotato un biglietto per l'evento a nome dell'utente indicato.
                    comando.CommandText =
                      "SELECT * " +
                      "FROM biglietto JOIN partita " +
                      "WHERE biglietto.Partita = partita.IDPartita " +
                      "AND partita.SquadraOUT = '" + sceltaposto.squadra + "' " +
                      "AND partita.Competizione = '" + sceltaposto.competizione + "' " +
                      "AND biglietto.Possessore = '" + sceltaposto.Tessera + "' ";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        conn.Close();
                        trovato.possessore = 0;
                        return trovato;
                    }
                    else
                        trovato.possessore = sceltaposto.Tessera;
                }
                conn.Close();
                conn.Open();

                using (MySqlCommand comando1 = conn.CreateCommand())
                {
                    //controllo nel database se ci sono posti liberi nel settore scelto e per la partita indicata.
                    comando1.CommandText =
                      "SELECT composizione.Importo, partita.IDPartita, partita.DataInizio, composizione.PostiLib, settore.Abbreviazione " +
                      "FROM composizione JOIN partita JOIN settore " +
                      "WHERE partita.IDPartita = composizione.CodPartita " +
                      "AND composizione.Settore = settore.Nome " +
                      "AND partita.SquadraOUT = '" + sceltaposto.squadra + "' " +
                      "AND partita.Competizione = '" + sceltaposto.competizione + "' " +
                      "AND composizione.Settore = '" + sceltaposto.settore + "' " +
                      "AND composizione.PostiLib > 0  ";
                    var result = comando1.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            trovato.settore = sceltaposto.settore;
                            trovato.importo = Convert.ToDecimal(result["Importo"].ToString());
                            trovato.CodPartita = Convert.ToInt32(result["IDPartita"]);
                            trovato.dataPartita = result["DataInizio"].ToString();
                            trovato.PostiLib = Convert.ToInt32(result["PostiLib"]);
                            trovato.SetAbb = result["Abbreviazione"].ToString();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nessun posto disponibile");
                        trovato.PostiLib = 0;
                        return trovato;
                    }
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comando2 = conn.CreateCommand())
                {
                    //cerco se il posto richiesto è disponibile.
                    comando2.CommandText =
                        "SELECT biglietto.posto " +
                        "FROM biglietto JOIN partita " +
                        "WHERE biglietto.Partita = partita.IDPartita " +
                        "AND partita.competizione = '" + sceltaposto.competizione + "' " +
                        "AND partita.SquadraOut = '" + sceltaposto.squadra + "' " +
                        "AND biglietto.settore = '" + sceltaposto.settore + "' " +
                        "AND biglietto.Posto = '" + sceltaposto.posto + "' ";
                    var resultposto = comando2.ExecuteReader();
                    if (resultposto.HasRows)
                    {
                        conn.Close();
                        Console.WriteLine("il posto scelto è occupato. ");
                        trovato.posto = 0;
                        return trovato;

                    }
                    else
                    {
                        trovato.posto = sceltaposto.posto;
                        trovato.Ospiti = sceltaposto.squadra;
                        trovato.CompPartita = sceltaposto.competizione;
                        conn.Close();
                        return trovato;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public Biglietto TrovaPostoPrezzo(SceltaPrezzo sceltaprezzo)
        {
            try
            {
                Biglietto biglietto = new Biglietto();
                Biglietto trovato = new Biglietto();

                List<Biglietto> SetDisp = new List<Biglietto>();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                decimal prezzo = sceltaprezzo.prezzo / sceltaprezzo.sconto;
                Console.WriteLine(prezzo);

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo, tramite il numero della tessera, se è già stato prenotato un biglietto per l'evento a nome dell'utente indicato.
                    comando.CommandText =
                      "SELECT * " +
                      "FROM biglietto JOIN partita " +
                      "WHERE biglietto.Partita = partita.IDPartita " +
                      "AND partita.SquadraOUT = '" + sceltaprezzo.squadra + "' " +
                      "AND partita.Competizione = '" + sceltaprezzo.competizione + "' " +
                      "AND biglietto.Possessore = '" + sceltaprezzo.Tessera + "' ";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        conn.Close();
                        trovato.possessore = 0;
                        return trovato;
                    }
                    else
                        trovato.possessore = sceltaprezzo.Tessera;
                }
                conn.Close();
                conn.Open();

                using (MySqlCommand comando1 = conn.CreateCommand())
                {
                    //controllo se è presente a database, per l'evento indicato, uno o più settori con postidisponibili e prezzo inferiore a quello richiesto dall'utente.
                    comando1.CommandText =
                      "SELECT settore.Nome, composizione.Importo, partita.IDPartita, partita.DataInizio, composizione.PostiLib, settore.Abbreviazione " +
                      "FROM composizione JOIN partita JOIN settore " +
                      "WHERE partita.IDPartita = composizione.CodPartita " +
                      "AND composizione.Settore = settore.Nome " +
                      "AND partita.SquadraOUT = '" + sceltaprezzo.squadra + "' " +
                      "AND partita.Competizione = '" + sceltaprezzo.competizione + "' " +
                      "AND composizione.Importo <= '" + prezzo + "' " +
                      "AND composizione.PostiLib > 0 " +
                      "ORDER BY composizione.Importo ";
                    var result = comando1.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            biglietto.settore = result["Nome"].ToString();
                            biglietto.importo = Convert.ToDecimal(result["Importo"].ToString()) * sceltaprezzo.sconto;
                            biglietto.CodPartita = Convert.ToInt32(result["IDPartita"]);
                            biglietto.dataPartita = result["DataInizio"].ToString();
                            biglietto.PostiLib = Convert.ToInt32(result["PostiLib"]);
                            biglietto.SetAbb = result["Abbreviazione"].ToString();
                            biglietto.Ospiti = sceltaprezzo.squadra;
                            biglietto.CompPartita = sceltaprezzo.competizione;
                            biglietto.posto = 0;
                            SetDisp.Add(biglietto);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nessun posto disponibile. ");
                        trovato.PostiLib = 0;
                        return trovato;
                    }
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comando2 = conn.CreateCommand())
                {
                    //Partendo dalla lista dei settori trovati nella richiesta precedente, cerco di assegnare un posto all'utente.
                    if (SetDisp.Count > 0)
                    {
                        foreach (var x in SetDisp)
                        {
                            int i = 1;
                            while (biglietto.posto == 0)
                            {
                                comando2.CommandText =
                                "SELECT biglietto.posto " +
                                "FROM biglietto JOIN partita " +
                                "WHERE biglietto.Partita = partita.IDPartita " +
                                "AND partita.competizione = '" + sceltaprezzo.competizione + "' " +
                                "AND partita.SquadraOut = '" + sceltaprezzo.squadra + "' " +
                                "AND biglietto.settore = '" + biglietto.settore + "' " +
                                "AND biglietto.Posto = '" + i + "' ";
                                var resultposto = comando2.ExecuteReader();
                                if (resultposto.HasRows)
                                {
                                    i++;
                                    comando2.Dispose();
                                    conn.Close();
                                    conn.Open();
                                }
                                else
                                {
                                    trovato.CodPartita = x.CodPartita;
                                    trovato.settore = x.settore;
                                    trovato.posto = i;
                                    trovato.importo = x.importo;
                                    trovato.dataPartita = x.dataPartita;
                                    trovato.PostiLib = x.PostiLib;
                                    trovato.SetAbb = x.SetAbb;
                                    trovato.Ospiti = x.Ospiti;
                                    trovato.CompPartita = x.CompPartita;
                                    SetDisp.Clear();
                                    conn.Close();
                                    conn.Open();
                                    return trovato;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public decimal ApplicaSconto(Biglietto biglietto)
        {
            try
            {
                Utente utente = new Utente();

                Sconto sconto = new Sconto();

                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando2 = conn.CreateCommand())
                {
                    //ottengo da database i dati che mi servono per conoscere la scontistica da applicare all'utente indicato.
                    comando2.CommandText =
                            "SELECT utente.DataNascita, utente.Sesso " +
                            "FROM utente " +
                            "WHERE utente.Id = '" + biglietto.possessore + "' ";
                    var resultutente = comando2.ExecuteReader();

                    if (resultutente.HasRows)
                    {
                        while (resultutente.Read())
                        {
                            utente.dataNascita = Convert.ToDateTime(resultutente["DataNascita"]).Date;
                            utente.sesso = resultutente["Sesso"].ToString();
                        }
                    }
                }
                conn.Close();
                conn.Open();
                using (MySqlCommand comando3 = conn.CreateCommand())
                {
                    //ottengo la tabella degli sconti che sono validi al momento e le età per accedervi.
                    comando3.CommandText =
                                "SELECT MAX(sconto.Under) AS Under, MAX(sconto.ScontoUnder) AS ScontoUnder, MAX(sconto.Anziano)AS Anziano, MAX(sconto.ScontoOver) AS ScontoOver, MAX(sconto.ScontoDonna) AS Donna " +
                                "FROM sconto " +
                                "WHERE sconto.Inizio <= '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' " +
                                "AND sconto.Fine >= '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' ";
                    var resultSconto = comando3.ExecuteReader();

                    if (resultSconto.HasRows)
                    {
                        while (resultSconto.Read())
                        {
                            sconto.giovane = Convert.IsDBNull(resultSconto["Under"]) ? 0 : (int)resultSconto["Under"];
                            sconto.scontoUnder = Convert.IsDBNull(resultSconto["ScontoUnder"]) ? 0 : (int)resultSconto["ScontoUnder"];
                            sconto.anziano = Convert.IsDBNull(resultSconto["Anziano"]) ? 0 : (int)resultSconto["Anziano"];
                            sconto.scontoOver = Convert.IsDBNull(resultSconto["ScontoOver"]) ? 0 : (int)resultSconto["ScontoOver"];
                            sconto.scontoDonna = Convert.IsDBNull(resultSconto["Donna"]) ? 0 : (int)resultSconto["Donna"];
                        }
                        //applico gli sconti in base ai dati dell'utente, facendo attenzione ad applicare lo sconto più favorevole.
                        if (utente.sesso == "F")
                            if (utente.GetAge(DateTime.Now, utente.dataNascita) < sconto.giovane && sconto.scontoUnder > sconto.scontoDonna)
                            {
                                biglietto.importo = ((100 - sconto.scontoUnder) * biglietto.importo) / 100;
                            }
                            else if (utente.GetAge(DateTime.Now, utente.dataNascita) >= sconto.anziano && sconto.scontoOver > sconto.scontoDonna)
                            {
                                biglietto.importo = ((100 - sconto.scontoOver) * biglietto.importo) / 100;
                            }
                            else
                            {
                                biglietto.importo = ((100 - sconto.scontoDonna) * biglietto.importo) / 100;
                            }
                        else if (utente.GetAge(DateTime.Now, utente.dataNascita) < sconto.giovane)
                        {
                            biglietto.importo = ((100 - sconto.scontoUnder) * biglietto.importo) / 100;
                        }
                        else if (utente.GetAge(DateTime.Now, utente.dataNascita) >= sconto.anziano)
                        {
                            biglietto.importo = ((100 - sconto.scontoOver) * biglietto.importo) / 100;
                        }
                        else
                        {
                            biglietto.importo = biglietto.importo;
                        }
                        conn.Close();
                        return biglietto.importo;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return biglietto.importo;
        }

        public SceltaPartita NuovoBiglietto(Biglietto b)
        {
            SceltaPartita sceltapartita = new SceltaPartita();

            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Start a local transaction.
                    MySqlTransaction sqlTran = connection.BeginTransaction();

                    // Enlist a command in the current transaction.
                    MySqlCommand command = connection.CreateCommand();
                    command.Transaction = sqlTran;

                    try
                    {
                        //controllo se il biglietto per l'evento è ancora disponibile.
                        command.CommandText =
                            "SELECT * " +
                            "FROM biglietto " +
                            "WHERE biglietto.Partita = '" + b.CodPartita + "' " +
                            "AND biglietto.Settore = '" + b.settore + "' " +
                            "AND biglietto.Posto = '" + b.posto + "' ";

                        var resultSelect = command.ExecuteScalar();

                        if (resultSelect != null)
                        {
                            conn.Close();
                            Console.WriteLine("biglietto già prenotato. ");
                            sceltapartita.message = "biglietto già prenotato. ";
                            return sceltapartita;
                        }
                        if (b.ContoUtente >= b.importo)
                        {
                            //ottengo dati utili alla registrazione e alla creazione dell codice univoco da assegnare al biglietto
                            string lettere = "ABCDEFGHILMN";
                            if (b.CompPartita.ToUpper() == "AMICHEVOLE")
                            {
                                b.CompPartita = "A";
                            }
                            else if (b.CompPartita.ToUpper() == "SERIE B")
                            {
                                b.CompPartita = "B";
                            }
                            else
                            {
                                b.CompPartita = "C";
                            }

                            string codice = b.dataPartita.Substring(0, 2) + lettere[Convert.ToInt32(b.dataPartita.Substring(3, 2)) - 1] + b.dataPartita.Substring(8, 2) + b.CompPartita + b.Ospiti.Substring(0, 3).ToUpper() + b.SetAbb;
                            
                            if (b.posto.ToString().Length == 1)
                                codice = codice + "000" + b.posto.ToString();
                            else if (b.posto.ToString().Length == 2)
                                codice = codice + "00" + b.posto.ToString();
                            else if (b.posto.ToString().Length == 3)
                                codice = codice + "0" + b.posto.ToString();
                            else
                                codice = codice + b.posto.ToString();
                            //registro il biglietto.   
                            command.CommandText = "INSERT INTO biglietto VALUES('" + b.CodPartita + "', '" + b.possessore + "', '" + b.settore + "', '" + b.posto + "', '" + codice + "' , '" + b.importo.ToString().Replace(",", ".") + "')";
                            var resultInsert = command.ExecuteNonQuery();
                            if (resultInsert > 0)
                            {
                                Console.WriteLine("Compilazione del biglietto riuscita. ");

                                string conto = (b.ContoUtente - b.importo).ToString().Replace(",", ".");
                                b.ContoUtente = Convert.ToDecimal(conto.Replace(".", ","));
                                //aggiorno il saldo dell'utente che ha pagato.
                                command.CommandText = "UPDATE utente set Conto = '" + conto + "' WHERE Id = '" + b.Pagante + "'";
                                var resultUpdate = command.ExecuteNonQuery();

                                if (resultUpdate > 0)
                                {
                                    Console.WriteLine("Credito riscosso con successo. ");
                                    b.PostiLib--;
                                    //aggiorno il numero di posti disponibili per l'evento.
                                    command.CommandText = "UPDATE composizione set PostiLib = '" + b.PostiLib + "' WHERE CodPartita = '" + b.CodPartita + "' AND Settore = '" + b.settore + "'";
                                    var resultPosti = command.ExecuteNonQuery();
                                    if (resultPosti > 0)
                                    {
                                        Console.WriteLine("Disponibilità posti aggiornata. ");
                                        sqlTran.Commit();
                                        sceltapartita.message = " ";
                                        return sceltapartita;
                                    }
                                    else
                                    {
                                        conn.Close();
                                        Console.WriteLine("Errore durante l'adeguamento della disponibilità posti. ");
                                        sceltapartita.message = "Errore lato server. ";
                                        return sceltapartita;
                                    }
                                }
                                else
                                {
                                    conn.Close();
                                    Console.WriteLine("Errore durante la riscossione dell'importo. ");
                                    sceltapartita.message = "Errore lato server. ";
                                    return sceltapartita;
                                }
                            }
                            else
                            {
                                conn.Close();
                                Console.WriteLine("Errore durante la creazione del biglietto. ");
                                sceltapartita.message = "Errore lato server. ";
                                return sceltapartita;
                            }
                        }
                        else
                        {
                            conn.Close();
                            Console.WriteLine("Credito dell'utente inferiore a quanto richiesto dall'operazione. ");
                            sceltapartita.message = "Credito dell'utente inferiore a quanto richiesto dall'operazione. ";
                            return sceltapartita;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception if the transaction fails to commit.
                        Console.WriteLine(ex.Message);

                        try
                        {
                            // Attempt to roll back the transaction.
                            sqlTran.Rollback();
                        }
                        catch (Exception exRollback)
                        {
                            // Throws an InvalidOperationException if the connection
                            // is closed or the transaction has already been rolled
                            // back on the server.
                            Console.WriteLine(exRollback.Message);
                        }
                    }
                }
                conn.Close();
                sceltapartita.message = "Errore lato server. ";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return sceltapartita;
        }

        public Utente BigliettiPassati(int idUtente)
        {
            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                Utente utente = new Utente();
                utente.listaBiglietti = new List<Biglietto>();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //ottengo i dati relativi ai biglietti prenotati dall'utente.
                    comando.CommandText =
                        "SELECT biglietto.codice, biglietto.settore, biglietto.posto, partita.Nome, partita.DataInizio,partita.OraInizio " +
                        "FROM biglietto JOIN Partita " +
                        "WHERE biglietto.Partita = partita.IDPartita " +
                        "AND biglietto.Possessore = '" + idUtente + "' ";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Biglietto b = new Biglietto();
                            b.codice = result["Codice"].ToString();
                            b.settore = result["Settore"].ToString();
                            b.posto = Convert.ToInt32(result["Posto"]);
                            b.dataPartita = result["DataInizio"].ToString().Substring(0,10) +" " +result["OraInizio"].ToString().Substring(0, 5);
                            b.nomePartita = result["Nome"].ToString();
                            utente.listaBiglietti.Add(b);
                        }
                    }
                    utente.id = idUtente;
                    return utente;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public Dictionary<string, int> Statistiche()
        {
            //metodo che ritorna il numero di biglietti venduti per ogni partita

            Dictionary<string, int> listaBiglietti = new Dictionary<string, int>();

            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    comando.CommandText = "SELECT Count(*) as numBiglietti,Partita.Nome FROM biglietto" +
                        "                   Join Partita on biglietto.Partita = Partita.IDPartita " +
                        "                   Group by partita.IDPartita";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            int numBiglietti = Convert.ToInt32(result["numBiglietti"]);
                            string nome = result["Nome"].ToString();
                            //inserisco nel dizionario il nome della partita e il numero di biglietti venduti
                            listaBiglietti.Add(nome, numBiglietti);
                        }
                        return listaBiglietti;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            listaBiglietti.Add("nulla", 0);

            return listaBiglietti;
        }

        public Dictionary<string, int> StatisticheEta(int idPartita)
        {
            //metodo che ritorna i biglietti per eta per una partita

            Dictionary<string, int> listaSort = new Dictionary<string, int>(); // dizionario che ritorno alla fine del metodo

            try
            {
                int presente = 0;  //variabile per indicare se una partita ha almeno un biglietto venduto
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                Dictionary<string, int> listaUtenti = new Dictionary<string, int>();

                try
                {
                    //fascia eta 0-18
                    using (MySqlCommand comando = conn.CreateCommand())
                    {
                        comando.CommandText = "SELECT DISTINCT Count(*) as numUtenti FROM biglietto" +
                            "                   Join Partita on biglietto.Partita = Partita.IDPartita " +
                            "                   Join Utente on Biglietto.Possessore = Utente.Id " +
                            "                   Where (2021 - YEAR(utente.DataNascita)) < 19 AND Partita.IDPartita = '" + idPartita + "'" +
                            "                   Group by partita.IDPartita";
                        var result = comando.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                int numBiglietti = Convert.ToInt32(result["numUtenti"]);
                                listaUtenti.Add(" Eta 0-18 ", numBiglietti);
                                presente = 1;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                conn.Close();
                conn.Open();
                try
                {
                    //fascia eta 18-30
                    using (MySqlCommand comando1 = conn.CreateCommand())
                    {
                        comando1.CommandText = "SELECT DISTINCT Count(*) as numUtenti FROM biglietto" +
                            "                   Join Partita on biglietto.Partita = Partita.IDPartita " +
                            "                   Join Utente on Biglietto.Possessore = Utente.Id " +
                            "                   Where (2021 - YEAR(utente.DataNascita)) < 31 AND (2021 - YEAR(utente.DataNascita)) > 18 AND Partita.IDPartita = '" + idPartita + "'" +
                            "                   Group by partita.IDPartita";
                        var result = comando1.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                int numBiglietti = Convert.ToInt32(result["numUtenti"]);
                                listaUtenti.Add(" Eta 18-30 ", numBiglietti);
                                presente = 1;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                conn.Close();
                conn.Open();
                try
                {
                    //fascia eta 30-60
                    using (MySqlCommand comando2 = conn.CreateCommand())
                    {
                        comando2.CommandText = "SELECT DISTINCT Count(*) as numUtenti FROM biglietto" +
                            "                   Join Partita on Biglietto.Partita = Partita.IDPartita " +
                            "                   Join Utente on Biglietto.Possessore = Utente.Id " +
                            "                   Where (2021 - YEAR(utente.DataNascita)) < 61 AND (2021 - YEAR(utente.DataNascita)) > 30 AND Partita.IDPartita = '" + idPartita + "'" +
                            "                   Group by partita.IDPartita";
                        var result = comando2.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                int numBiglietti = Convert.ToInt32(result["numUtenti"]);
                                listaUtenti.Add(" Eta 30-60 ", numBiglietti);
                                presente = 1;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                conn.Close();
                conn.Open();
                try
                {
                    //fascia eta over 60
                    using (MySqlCommand comando2 = conn.CreateCommand())
                    {
                        comando2.CommandText = "SELECT DISTINCT Count(*) as numUtenti FROM biglietto" +
                            "                   Join Partita on biglietto.Partita = Partita.IDPartita " +
                            "                   Join Utente on Biglietto.Possessore = Utente.Id " +
                            "                   Where (2021 - YEAR(utente.DataNascita)) > 60 AND Partita.IDPartita = '" + idPartita + "'" +
                            "                   Group by partita.IDPartita";
                        var result = comando2.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                int numBiglietti = Convert.ToInt32(result["numUtenti"]);
                                listaUtenti.Add(" Eta Over 60 ", numBiglietti);
                                presente = 1;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                conn.Close();

                //riempio il dizionario con i valori mancanti impostandoli a zero , per avere sempre 4 valori nel dizionario
                if (!(listaUtenti.ContainsKey(" Eta 0-18 ")) && presente > 0)
                {
                    listaUtenti.Add(" Eta 0-18 ", 0);
                }
                if (!listaUtenti.ContainsKey(" Eta 18-30 ") && presente > 0)
                {
                    listaUtenti.Add(" Eta 18-30 ", 0);
                }
                if (!listaUtenti.ContainsKey(" Eta 30-60 ") && presente > 0)
                {
                    listaUtenti.Add(" Eta 30-60 ", 0);
                }
                if (!listaUtenti.ContainsKey(" Eta Over 60 ") && presente > 0)
                {
                    listaUtenti.Add(" Eta Over 60 ", 0);
                }

                //ordino il dizionario in base alle fascie di eta
                foreach (KeyValuePair<string, int> statistica in listaUtenti.OrderBy(key => key.Key))
                {
                    listaSort.Add(statistica.Key, statistica.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return listaSort;
        }

        public List<Partita> PartiteTotali()
        {
            //metodo che ritorna tutte le partite
            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                List<Partita> listaPartita = new List<Partita>();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    comando.CommandText = "SELECT * FROM partita";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Partita p = new Partita();
                            p.id = Convert.ToInt32(result["IDPartita"]);
                            p.nome = result["Nome"].ToString();
                            p.squadra1 = result["SquadraIN"].ToString();
                            p.squadra2 = result["SquadraOUT"].ToString();
                            p.competizione = result["Competizione"].ToString();

                            listaPartita.Add(p);

                        }
                        return listaPartita;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public bool ModificaSconti(Sconto s)
        {
            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    if ((s.giovane > s.anziano && s.anziano > 0 && s.scontoOver > 0) || ((s.anziano > 0 && s.scontoOver == 0) || (s.anziano == 0 && s.scontoOver > 0)) || ((s.giovane > 0 && s.scontoUnder == 0) || (s.giovane == 0 && s.scontoUnder > 0)) || (s.scontoOver > 100 || s.scontoUnder > 100))
                    {
                        return false;
                    }
                    else
                    {
                        //aggiorno i valori della tabella sconti.
                        comando.CommandText =
                            "INSERT  INTO sconto VALUES ('" + s.giovane + "','" + s.scontoUnder + "','" + s.anziano + "','" + s.scontoOver + "','" + s.scontoDonna + "','" + s.inizio.ToString("yyyy-MM-dd") + "','" + s.fine.ToString("yyyy-MM-dd") + "')";
                        var resultInsert = comando.ExecuteNonQuery();

                        if (resultInsert > 0)
                        {
                            //aggiorno anche i valori che con l'aggiornamento risulterebbero obsoleti.
                            if (s.anziano > 0 && s.scontoOver > 0)
                            {
                                comando.CommandText =
                                    "UPDATE 'sconto' SET 'Anziano' = 0, 'ScontoOver' = 0 WHERE Anziano != '" + s.anziano + "' OR ScontoOver != '" + s.scontoOver + "' ";
                                var resultUpdate = comando.ExecuteNonQuery();
                                if (resultUpdate == 0)
                                {
                                    Console.WriteLine("Errore durante l'aggiornamento dello sconto");
                                    return false;
                                }
                            }
                            if (s.giovane > 0 && s.scontoUnder > 0)
                            {
                                comando.CommandText =
                                    "UPDATE sconto SET Under = 0, ScontoUnder = 0 WHERE Under != '" + s.giovane + "' OR ScontoUnder != '" + s.scontoUnder + "' ";
                                var resultUpdate = comando.ExecuteNonQuery();
                                if (resultUpdate == 0)
                                {
                                    Console.WriteLine("Errore durante l'aggiornamento dello sconto");
                                    return false;
                                }
                            }
                            if (s.scontoDonna > 0)
                            {
                                comando.CommandText =
                                    "UPDATE sconto SET ScontoDonna = 0 WHERE ScontoDonna != '" + s.scontoDonna + "' ";
                                var resultUpdate = comando.ExecuteNonQuery();
                                if (resultUpdate == 0)
                                {
                                        Console.WriteLine("Errore durante l'aggiornamento dello sconto");
                                        return false;
                                }
                            }
                            //cancello dal database i record che non mi forniscono più informazioni.
                            comando.CommandText =
                                "DELETE FROM sconto WHERE Anziano = 0 AND ScontoOver = 0 AND Under = 0 AND ScontoUnder = 0 AND ScontoDonna = 0";
                            var resultDelete = comando.ExecuteNonQuery();

                            Console.WriteLine("Aggiornamento della tabella sconti effettuato.");
                            return true;
                        }
                        if (resultInsert == 0)
                        {
                            Console.WriteLine("Errore durante l'inserimento dello sconto.");
                            return false;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public bool IsUserAvailable(Utente User)
        {
            try
            {
                MySqlConnection conn = null;
                var connectionString = "server=localhost;database=stadio;uid=root;pwd='';";

                conn = new MySqlConnection(connectionString);
                conn.Open();

                using (MySqlCommand comando = conn.CreateCommand())
                {
                    //controllo se l'usenrame è già presente a database.
                    comando.CommandText =
                        "SELECT * " +
                        "FROM utente " +
                        "WHERE username = '" + User.username + "'";
                    var result = comando.ExecuteReader();
                    if (result.HasRows)
                    {
                        Console.WriteLine("Username già in uso.");
                        return false;
                    }
                    else
                        return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }        
    }
}


