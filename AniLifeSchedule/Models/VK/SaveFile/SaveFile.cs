using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.SaveFile
{
    public class SaveFile
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("doc")]
        public SaveFileDocumentData Document { get; set; }
    }
}
