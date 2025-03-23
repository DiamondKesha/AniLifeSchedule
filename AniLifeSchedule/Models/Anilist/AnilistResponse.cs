using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist;

public class AnilistResponse<T> where T : Page
{
    [JsonPropertyName("Page")]
    public T Page { get; set; } = default!;
}
