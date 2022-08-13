using AniLifeSchedule.Data;

namespace AniLifeSchedule.ViewModels.Implementations
{
    public class CreatePostsViewModel : ICreatePostsViewModel
    {
        public ScheduleTransferData ScheduleData { get; }

        public CreatePostsViewModel(ScheduleTransferData scheduleData)
        {
            ScheduleData = scheduleData;
        }
    }
}
