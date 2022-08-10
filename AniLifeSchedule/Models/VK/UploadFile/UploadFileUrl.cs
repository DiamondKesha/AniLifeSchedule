using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.UploadResponse
{
    public class UploadFileUrl
    {
        [JsonPropertyName("upload_url")]
        public string File { get; set; } = string.Empty;
    }
}
