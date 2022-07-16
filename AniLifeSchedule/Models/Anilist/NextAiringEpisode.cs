using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{
    public class NextAiringEpisode
    {
        [JsonPropertyName("airingAt")]
        public long AiringAt { get; set; }

        [JsonPropertyName("episode")]
        public int? Episode { get; set; }

        public DateTime GetTime
        {
            get
            {
                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return time.AddSeconds(AiringAt).ToLocalTime();
            }
        }
    }
}
