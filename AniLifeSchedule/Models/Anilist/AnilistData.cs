using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{
    public class AnilistData
    {
        [JsonPropertyName("Page")]
        public Page Page { get; set; }
    }
}
