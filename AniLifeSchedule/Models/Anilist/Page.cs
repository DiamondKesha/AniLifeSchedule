using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{
    public class Page
    {
        [JsonPropertyName("airingSchedules")]
        public List<AiringSchedules> AiringSchedules { get; set; } = null!;
    }
}
