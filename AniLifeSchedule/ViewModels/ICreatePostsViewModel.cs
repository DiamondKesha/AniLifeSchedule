using AniLifeSchedule.Data;

namespace AniLifeSchedule.ViewModels
{
    public interface ICreatePostsViewModel
    {
        /// <summary>
        /// Schedule Data which should be getted from Schedule.razor page.
        /// </summary>
        public ScheduleTransferData ScheduleData { get; }
    }
}
