using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Shikimori;

public class Poster
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("originalUrl")]
    public string OriginalUrl { get; set; } = string.Empty;

    [JsonPropertyName("mainUrl")]
    public string MainUrl { get; set; } = string.Empty;
}
