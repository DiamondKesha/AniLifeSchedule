using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK;

public class ErrorData
{
    [JsonPropertyName("error_code")]
    public int Id { get; set; }

    [JsonPropertyName("error_msg")]
    public string Message { get; set; } = string.Empty;
}
