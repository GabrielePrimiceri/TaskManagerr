using Newtonsoft.Json;

namespace Applicazione.Dto;

public record AttivitaDto(
    [property: JsonProperty("Nome")] string Nome,
    [property: JsonProperty("Durata")] double Durata,
    [property: JsonProperty("StatoAttivita")] StatoDto StatoAttivita);
