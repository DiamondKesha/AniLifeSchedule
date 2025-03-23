using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Photo;

public class PhotoSaveWallPhotoResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("owner_id")]
    public long OwnerId { get; set; }

    [JsonPropertyName("access_key")]
    public string AccessKey { get; set; } = string.Empty;
}
