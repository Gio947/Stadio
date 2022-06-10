using stadio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFStadio
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        Utente Login(Account account);

        [OperationContract]
        Utente Registrazione(Utente utente);

        [OperationContract]
        bool AggiungiPartita(Partita p);

        [OperationContract]
        Utente AggiornaConto(Utente u);

        [OperationContract]
        Biglietto TrovaPostoPartita(SceltaPartita sceltapartita);

        [OperationContract]
        Biglietto TrovaPostoSettore(SceltaSettore sceltasettore);

        [OperationContract]
        Biglietto TrovaPostoPrezzo(SceltaPrezzo sceltaprezzo);

        [OperationContract]
        Biglietto TrovaPostoPosto(SceltaPosto sceltaposto);

        [OperationContract]
        decimal ApplicaSconto(Biglietto biglietto);

        [OperationContract]
        SceltaPartita NuovoBiglietto(Biglietto biglietto);

        [OperationContract]
        Dictionary<string,int> Statistiche();

        [OperationContract]
        Dictionary<string, int> StatisticheEta(int idPartita);

        [OperationContract]
        List<Partita> PartiteTotali();

        [OperationContract]
        Utente BigliettiPassati(int idUtente);

        [OperationContract]
        bool ModificaSconti(Sconto s);

        [OperationContract]
        bool IsUserAvailable(Utente User);
    }
}
