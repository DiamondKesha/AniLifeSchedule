using AniLifeSchedule.Models.Enums;
using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Shikimori;

public class Animes
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Romaji { get; set; } = string.Empty;

    [JsonPropertyName("russian")]
    public string Russian { get; set; } = string.Empty;

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("episodesAired")]
    public int? EpisodesAired { get; set; }

    [JsonPropertyName("poster")]
    public Poster Poster { get; set; } = default!;

    [JsonPropertyName("nextEpisodeAt")]
    public DateTime? NextEpisodeReleaseDate { get; set; }

    public StatusType GetStatusEnum()
    {
        return Status switch
        {
            "anons" => StatusType.NOT_YET_RELEASED,
            "ongoing" => StatusType.RELEASING,
            "released" => StatusType.FINISHED,
            _ => StatusType.RELEASING
        };
    }

    public FormatType GetFormatEnum()
    {
        return Kind switch
        {
            "tv" => FormatType.TV,
            "movie" => FormatType.MOVIE,
            _ => FormatType.TV,
        };
    }
}
