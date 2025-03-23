using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist.MediaData;

public class Title
{
    [JsonPropertyName("romaji")]
    public string Romaji { get; set; } = string.Empty;

    [JsonPropertyName("english")]
    public string English { get; set; } = string.Empty;

    [JsonPropertyName("native")]
    public string Native { get; set; } = string.Empty;
}
