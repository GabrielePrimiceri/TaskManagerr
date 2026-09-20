using Newtonsoft.Json;

namespace Applicazione.Dto;

public record StatoDto([property: JsonProperty("Stato")] string Stato);
