using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Photo;

public class PhotoUploadedToServerResponse
{
    [JsonPropertyName("server")]
    public int ServerId { get; set; }

    [JsonPropertyName("photo")]
    public string Photo { get; set; } = string.Empty;

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;
}
