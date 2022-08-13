using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Wrappers
{
    public class ResponseWrapper<T> where T : class
    {
        [JsonPropertyName("response")]
        public T Response { get; set; }
    }
}
