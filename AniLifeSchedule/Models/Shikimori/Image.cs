using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Shikimori
{
    public class Image
    {
        private string _original;

        [JsonPropertyName("original")]
        public string Original { get => _original; set => _original = $"https://shikimori.one/{value}"; }
    }
}
