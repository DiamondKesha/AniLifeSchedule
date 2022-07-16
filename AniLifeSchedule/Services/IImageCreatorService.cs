using AniLifeSchedule.Models;

namespace AniLifeSchedule.Services
{
    public interface IImageCreatorService
    {
        /// <summary>
        /// Generating image and return base64 type string
        /// </summary>
        /// <param name="scheduleModel">Schedule Data</param>
        /// <returns></returns>
        public Task<string> Create(List<ScheduleModel> scheduleModel);
    }
}
