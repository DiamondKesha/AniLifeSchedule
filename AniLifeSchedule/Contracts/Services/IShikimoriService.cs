using AniLifeSchedule.Models.Shikimori;

namespace AniLifeSchedule.Contracts.Services;

public interface IShikimoriService
{
    public Task<ErrorOr<ShikimoriResponse>> GetAnimeByTitle(string title);

    public Task<ErrorOr<ShikimoriResponse>> GetAnimeByIds(string ids);
}
