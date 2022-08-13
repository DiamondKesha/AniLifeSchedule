using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.WallPost
{
    public class WallPost
    {
        [JsonPropertyName("post_id")]
        public int PostId { get; set; }
    }
}
