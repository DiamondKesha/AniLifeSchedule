using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist;

public class Page
{
    [JsonPropertyName("pageInfo")]
    public PageInfo PageInfo { get; set; } = default!;
}

public class PageInfo
{
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }
}
