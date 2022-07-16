using AniLifeSchedule.Models.Anilist;

namespace AniLifeSchedule.Services
{
    public interface IAnilistService
    {
        /// <summary>
        /// Gets schedule by date
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>AnilistData object</returns>
        public Task<AnilistData?> GetScheduleByDate(DateTime date);
    }
}
