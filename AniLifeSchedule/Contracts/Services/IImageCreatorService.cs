namespace AniLifeSchedule.Contracts.Services;

public interface IImageCreatorService
{
    public Task<string> CreateScheduleImage(List<ScheduleModel> scheduleModels);
}
