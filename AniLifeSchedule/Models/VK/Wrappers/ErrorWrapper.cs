using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Wrappers;

public class ErrorWrapper<T> where T : class
{
    [JsonPropertyName("error")]
    public T Error { get; set; } = default!;
}
