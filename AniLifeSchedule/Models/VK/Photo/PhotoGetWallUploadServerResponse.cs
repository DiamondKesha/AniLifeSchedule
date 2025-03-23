using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Photo;

public class PhotoGetWallUploadServerResponse
{
    [JsonPropertyName("upload_url")]
    public string Url { get; set; } = string.Empty;
}
