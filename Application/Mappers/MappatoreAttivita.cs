using Applicazione.Dto;
using Dominio.Modello.Entita;

namespace Applicazione.Mappatori;

public static class MappatoreAttivita
{
    public static AttivitaDto ToDto(this AttivitaSistema attivita) =>
        new(attivita.Nome, attivita.Durata, attivita.StatoAttivita.ToDto());

    public static AttivitaSistema ToEntity(this AttivitaDto attivitaDto) =>
        new(attivitaDto.Nome, attivitaDto.Durata, attivitaDto.StatoAttivita.ToEntity());
}
