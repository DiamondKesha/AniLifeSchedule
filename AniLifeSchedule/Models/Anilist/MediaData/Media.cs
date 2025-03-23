using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist.MediaData;

public class Media
{
    [JsonPropertyName("nextAiringEpisode")]
    public NextAiringEpisode NextAiringEpisode { get; set; } = default!;

    [JsonPropertyName("title")]
    public Title Title { get; set; } = default!;

    [JsonPropertyName("coverImage")]
    public CoverImage Cover { get; set; } = default!;

    [JsonPropertyName("startDate")]
    public Date StartDate { get; set; } = default!;

    [JsonPropertyName("endDate")]
    public Date? EndDate { get; set; }

    [JsonPropertyName("isAdult")]
    public bool IsAdult { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("studios")]
    public Studios Studios { get; set; } = default!;
}
