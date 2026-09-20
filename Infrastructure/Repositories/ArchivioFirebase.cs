using Applicazione.Dto;
using Applicazione.Interfacce;
using Applicazione.Mappatori;
using Dominio.Modello.Entita;
using Dominio.Modello.OggettiValore;
using Firebase.Database;
using Firebase.Database.Query;

namespace Infrastruttura.Archivi;

public class ArchivioFirebase : IArchivioAttivita
{
    private const string NodoAttivita = "tasks";
    private readonly FirebaseClient _client;

    public ArchivioFirebase(string indirizzoFirebase) => _client = new FirebaseClient(indirizzoFirebase);

    public async Task<IEnumerable<AttivitaSistema>> OttieniTutteLeAttivitaAsync()
    {
        var elementiDto = await _client.Child(NodoAttivita).OnceAsync<AttivitaDto>();
        return elementiDto.Select(elemento => elemento.Object.ToEntity()).ToList();
    }

    public async Task<AttivitaSistema> OttieniAttivitaPerNomeAsync(string nome)
    {
        var attivitaDto = await _client.Child(NodoAttivita).Child(nome).OnceSingleAsync<AttivitaDto>();
        return attivitaDto.ToEntity();
    }

    public async Task<IEnumerable<AttivitaSistema>> OttieniAttivitaPerStatoAsync(Stato stato)
    {
        var attivita = await OttieniTutteLeAttivitaAsync();
        return attivita.Where(elemento => elemento.StatoAttivita == stato).ToList();
    }

    public async Task SalvaAttivitaAsync(AttivitaSistema attivita) =>
        await _client.Child(NodoAttivita).Child(attivita.Nome).PutAsync(attivita.ToDto());
}
