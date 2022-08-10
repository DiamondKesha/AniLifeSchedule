using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.UploadResponse
{
    public class UploadFileData
    {
        [JsonPropertyName("file")]
        public string File { get; set; } = String.Empty;

        [JsonPropertyName("error")]
        public string Error { get; set; } = String.Empty;

        [JsonPropertyName("error_descr")]
        public string ErrorDescription { get; set; } = String.Empty;
    }
}
