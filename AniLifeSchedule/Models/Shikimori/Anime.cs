using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Shikimori
{
    public class Anime
    {
        /// <summary>
        /// Russian Title
        /// </summary>
        [JsonPropertyName("russian")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Romaji Title
        /// </summary>
        [JsonPropertyName("name")]
        public string TitleRomaji { get; set; } = string.Empty;

        /// <summary>
        /// Images
        /// </summary>
        [JsonPropertyName("image")]
        public Image Image { get; set; }

        /// <summary>
        /// Episode aired
        /// </summary>
        [JsonPropertyName("episodes_aired")]
        public int? Episode { get; set; }

        /// <summary>
        /// Episodes
        /// </summary>
        [JsonPropertyName("episodes")]
        public int? Episodes { get; set; }

        /// <summary>
        /// Format
        /// </summary>
        [JsonPropertyName("kind")]
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
