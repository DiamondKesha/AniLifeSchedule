using AniLifeSchedule.Models.Enums;

namespace AniLifeSchedule.Models;

public class ScheduleModel(
    TitleModel titles,
    string imageUrl,
    string currentAiringEpisode, DateTime episodeReleaseDate, int? episodes,
    FormatType format, StatusType status)
{
    public TitleModel Titles { get; set; } = titles;

    public string ImageUrl { get; set; } = imageUrl;

    public string CurrentAiringEpisode { get; set; } = currentAiringEpisode;
    public DateTime EpisodeReleaseDate { get; set; } = episodeReleaseDate;
    public int? Episodes { get; set; } = episodes;

    public FormatType Format { get; set; } = format;
    public StatusType Status { get; set; } = status;

    public static ScheduleModel Create(TitleModel titles, string imageUrl, string currentAiringEpisode, DateTime episodeReleaseDate, int? episodes, FormatType format, StatusType status)
    {
        return new(titles, imageUrl, currentAiringEpisode, episodeReleaseDate, episodes, format, status);
    }
}
