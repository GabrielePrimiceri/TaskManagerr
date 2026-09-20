using Dominio.Modello.Entita;
using Dominio.Modello.OggettiValore;

namespace Applicazione.Interfacce;

public interface IArchivioAttivita
{
    Task SalvaAttivitaAsync(AttivitaSistema attivita);
    Task<IEnumerable<AttivitaSistema>> OttieniTutteLeAttivitaAsync();
    Task<IEnumerable<AttivitaSistema>> OttieniAttivitaPerStatoAsync(Stato stato);
    Task<AttivitaSistema> OttieniAttivitaPerNomeAsync(string nome);
}
