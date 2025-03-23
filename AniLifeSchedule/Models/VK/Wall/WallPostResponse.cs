using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Wall;

public class WallPostResponse
{
    [JsonPropertyName("post_id")]
    public int PostId { get; set; }
}
