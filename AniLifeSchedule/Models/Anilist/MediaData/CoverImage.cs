using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist.MediaData;

public class CoverImage
{
    [JsonPropertyName("large")]
    public string Large { get; set; } = string.Empty;
}
