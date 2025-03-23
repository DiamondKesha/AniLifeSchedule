using AniLifeSchedule.Models.Anilist.MediaData;
using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.Anilist;

public class AnilistAiringScheduleResponse : Page
{
    [JsonPropertyName("airingSchedules")]
    public List<AiringSchedules> AiringSchedule { get; set; } = null!;
}
