using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.SaveFile
{
    public class SaveFileDocumentData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("owner_id")]
        public long OwnerId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}