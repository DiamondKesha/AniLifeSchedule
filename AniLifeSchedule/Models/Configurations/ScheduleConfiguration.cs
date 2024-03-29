﻿namespace AniLifeSchedule.Models.Configurations
{
    public class ScheduleConfiguration
    {
        public string PathToSave { get; set; } = string.Empty;

        public string TimeToPost { get; set; } = string.Empty;

        public string Filename { get;set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string FileTags { get; set; } = string.Empty;

        public bool DeleteVkDocumentAfterPost { get; set; }

        public bool DeleteFileAfterPost { get; set; }
    }
}
