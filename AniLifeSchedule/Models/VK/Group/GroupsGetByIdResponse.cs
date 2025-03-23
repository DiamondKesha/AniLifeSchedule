using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.Group;

public class GroupsGetByIdResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("members_count")]
    public int MembersCount { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("photo_50")]
    public string Photo50 { get; set; } = string.Empty;

    [JsonPropertyName("photo_100")]
    public string Photo100 { get; set; } = string.Empty;

    [JsonPropertyName("photo_200")]
    public string Photo200 { get; set; } = string.Empty;
}
