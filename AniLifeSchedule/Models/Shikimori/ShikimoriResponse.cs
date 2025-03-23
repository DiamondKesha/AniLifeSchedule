using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Shikimori;

public class ShikimoriResponse
{
    [JsonPropertyName("animes")]
    public List<Animes> Animes { get; set; } = default!;
}
