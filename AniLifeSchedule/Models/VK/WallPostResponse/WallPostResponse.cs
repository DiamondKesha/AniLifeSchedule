using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.WallPostResponse
{
    public class WallPostResponse
    {
        [JsonPropertyName("post_id")]
        public int PostId { get; set; }
    }
}
