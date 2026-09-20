using Applicazione.Dto;
using Applicazione.Interfacce;
using Applicazione.Mappatori;
using Dominio.Modello.Entita;
using Dominio.Modello.OggettiValore;

namespace Applicazione.CasiUso;

public class ServizioGestoreAttivita : IServizioGestoreAttivita
{
    private readonly IArchivioAttivita _archivioAttivita;

    public ServizioGestoreAttivita(IArchivioAttivita archivioAttivita) => _archivioAttivita = archivioAttivita;

    public async Task CreaAttivitaAsync(AttivitaDto attivitaDto)
    {
        var attivita = attivitaDto.ToEntity();
        await _archivioAttivita.SalvaAttivitaAsync(attivita);
    }

    public async Task<IEnumerable<AttivitaDto>> OttieniTutteLeAttivitaAsync() =>
        (await _archivioAttivita.OttieniTutteLeAttivitaAsync()).Select(attivita => attivita.ToDto());

    public async Task AvviaTutteLeAttivitaAsync()
    {
        var attivita = (await _archivioAttivita.OttieniTutteLeAttivitaAsync())
            .Where(elemento => elemento.StatoAttivita == Stato.IN_ATTESA);
        await Task.WhenAll(attivita.Select(AvviaESalvaAsync));
    }

    public async Task AvviaSingolaAttivitaAsync(AttivitaDto attivitaDto)
    {
        var attivita = await _archivioAttivita.OttieniAttivitaPerNomeAsync(attivitaDto.Nome);
        await AvviaESalvaAsync(attivita);
    }

    private async Task AvviaESalvaAsync(AttivitaSistema attivita)
    {
        var esecuzione = attivita.AvviaAsync();
        await _archivioAttivita.SalvaAttivitaAsync(attivita);
        await esecuzione;
        await _archivioAttivita.SalvaAttivitaAsync(attivita);
    }
}
