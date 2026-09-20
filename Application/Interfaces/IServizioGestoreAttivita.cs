using Applicazione.Dto;

namespace Applicazione.Interfacce;

public interface IServizioGestoreAttivita
{
    Task CreaAttivitaAsync(AttivitaDto attivitaDto);
    Task AvviaSingolaAttivitaAsync(AttivitaDto attivitaDto);
    Task AvviaTutteLeAttivitaAsync();
    Task<IEnumerable<AttivitaDto>> OttieniTutteLeAttivitaAsync();
}
