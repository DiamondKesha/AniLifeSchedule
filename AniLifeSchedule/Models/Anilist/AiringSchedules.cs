using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{
    public class AiringSchedules
    {
        [JsonPropertyName("media")]
        public Media Media { get; set; }
    }
}
