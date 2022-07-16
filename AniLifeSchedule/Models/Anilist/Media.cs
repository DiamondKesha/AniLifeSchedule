using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{
    public class Media
    {
        [JsonPropertyName("nextAiringEpisode")]
        public NextAiringEpisode NextAiringEpisode { get; set; } 

        [JsonPropertyName("title")]
        public Title Title { get; set; }

        [JsonPropertyName("coverImage")]
        public CoverImage Cover { get; set; }

        [JsonPropertyName("startDate")]
        public Date StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public Date? EndDate { get; set; }

        [JsonPropertyName("isAdult")]
        public bool IsAdult { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }
    }
}
