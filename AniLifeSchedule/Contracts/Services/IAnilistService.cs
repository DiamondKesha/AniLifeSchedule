using AniLifeSchedule.Models.Anilist;

namespace AniLifeSchedule.Contracts.Services;

public interface IAnilistService
{
    public Task<ErrorOr<AnilistResponse<AnilistAiringScheduleResponse>>> GetScheduleByDate(DateTime date, int? page = null);
}
