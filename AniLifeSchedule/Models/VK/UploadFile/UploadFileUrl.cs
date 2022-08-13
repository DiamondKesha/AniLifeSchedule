using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.UploadFile
{
    public class UploadFileUrl
    {
        [JsonPropertyName("upload_url")]
        public string Url { get; set; } = string.Empty;
    }
}
