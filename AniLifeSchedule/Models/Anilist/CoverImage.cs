using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{
    public class CoverImage
    {
        [JsonPropertyName("large")]
        public string Large { get; set; }
    }
}
