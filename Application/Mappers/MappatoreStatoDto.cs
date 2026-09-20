using Applicazione.Dto;
using Dominio.Modello.OggettiValore;

namespace Applicazione.Mappatori;

public static class MappatoreStatoDto
{
    public static Stato ToEntity(this StatoDto statoDto) => statoDto.Stato.ToLowerInvariant() switch
    {
        "waiting" or "in_attesa" => Stato.IN_ATTESA,
        "in_progress" or "in_corso" => Stato.IN_CORSO,
        "completed" or "completata" => Stato.COMPLETATA,
        _ => throw new ArgumentException("Il valore dello stato non e valido.")
    };

    public static StatoDto ToDto(this Stato stato) => new(stato.ToString());
}
