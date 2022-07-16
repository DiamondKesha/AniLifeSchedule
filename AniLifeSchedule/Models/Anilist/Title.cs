using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist
{ 
    public class Title
    {
        [JsonPropertyName("romaji")]
        public string Romaji { get; set; } = null!;

        [JsonPropertyName("english")]
        public string English { get; set; } = null!;

        [JsonPropertyName("native")]
        public string Native { get; set; } = null!;
    }
}
