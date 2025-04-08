using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist.MediaData;

public class CoverImage
{
    [JsonPropertyName("large")]
    public string Large { get; set; } = string.Empty;

    [JsonPropertyName("extraLarge")]
    public string ExtraLarge { get; set; } = string.Empty;
}
