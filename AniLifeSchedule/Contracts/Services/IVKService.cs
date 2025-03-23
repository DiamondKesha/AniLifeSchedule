using AniLifeSchedule.Models.VK.Group;
using AniLifeSchedule.Models.VK.Wall;

namespace AniLifeSchedule.Contracts.Services;

public interface IVKService
{    public Task<ErrorOr<List<GroupsGetByIdResponse>>> GetGroupInformation();

    public Task<ErrorOr<WallPostResponse>> CreatePost(string text, string attachments, DateTime? time = null);

    public Task<ErrorOr<string>> UploadPhotoToServer(string photoPath);
}
