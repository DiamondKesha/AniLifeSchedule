using AniLifeSchedule.Enums;

namespace AniLifeSchedule.Models
{
    public class ScheduleModel
    {
        public string ImageUrl { get; set; } = string.Empty;

        public string TitleRussian { get; set; } = string.Empty;
        public string TitleRomaji { get; set; } = string.Empty;

        public string CurrentEpisode { get; set; } = string.Empty;
        public int? Episodes { get; set; }
        
        public DateTime ReleaseDate { get; set; }

        public Format Format { get; set; }
        public Status Status { get; set; }

        public bool IsEditable { get; set; } = false;
    }
}
