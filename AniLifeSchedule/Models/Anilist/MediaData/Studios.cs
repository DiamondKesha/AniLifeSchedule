using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist.MediaData;

public class Studios
{
    [JsonPropertyName("edges")]
    public List<Edge> Edges { get; set; } = new();
}

public class Edge
{
    [JsonPropertyName("node")]
    public Node Node { get; set; } = default!;
}

public class Node
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("isAnimationStudio")]
    public bool IsAnimationStudio { get; set; }
}
