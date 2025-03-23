using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist;

public class AnilistMediaResponse : Page
{
    [JsonPropertyName("media")]
    public List<MediaData.Media> Media { get; set; } = null!;
}
