using AniLifeSchedule.Common.Base;
using AniLifeSchedule.Common.Extensions;
using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Models.VK.Group;
using AniLifeSchedule.Models.VK.Photo;
using AniLifeSchedule.Models.VK.Wall;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace AniLifeSchedule.Services.VK;

public partial class VKService(IVKAuthService vKAuthService, IHttpClientFactory httpClientFactory, IOptions<VKConfiguration> vkConfig) : IVKService
{
    private readonly IVKAuthService _vkAuthService = vKAuthService;

    private HttpClient _httpClient { get; set; } = httpClientFactory.CreateClient("VK");

    private readonly VKConfiguration _vkConfig = vkConfig.Value;

    public async Task<ErrorOr<List<GroupsGetByIdResponse>>> GetGroupInformation()
    {
        var result = await SendAsync($"groups.getById?access_token=TOKEN&fields=name,members_count&group_id={_vkConfig.GroupId}&v={_vkConfig.ApiVersion}");

        return await result.GetData<List<GroupsGetByIdResponse>>(true, true);
    }

    public async Task<ErrorOr<WallPostResponse>> CreatePost(string text, string attachments, DateTime? time = null)
    {
        string urlApi = $"wall.post?access_token=TOKEN&owner_id=-{_vkConfig.GroupId}&message={text}&attachments={attachments}&v={_vkConfig.ApiVersion}";

        if (time != null && time >= DateTime.Now) urlApi += $"&publish_date={new DateTimeOffset(time.Value).ToUnixTimeSeconds()}";

        var createPost = await SendAsync(urlApi);
        var createPostData = await createPost.GetData<WallPostResponse>(true, true);

        if (createPostData.IsError) return Error.Failure("Error.VK", "Failed to create a new post." + createPostData.FirstError.Description);

        return createPostData.Value;
    }

    public async Task<ErrorOr<string>> UploadPhotoToServer(string photoPath)
    {
        var urlUploadServer = await GetWallPhotoUploadServer();

        if (urlUploadServer.IsError) return Error.Failure("Error.VK", "Failed to retrieve the upload link for the photo: " + urlUploadServer.FirstError.Description);

        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(photoPath);
        using var content = new StreamContent(fileStream);

        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(content, "photo", "schedule.jpg");

        var uploadedPhoto = await SendAsync(urlUploadServer.Value.Url, form);
        var uploadPhotoData = await uploadedPhoto.GetData<PhotoUploadedToServerResponse>(false, false);

        if (uploadPhotoData.IsError) return Error.Failure("Error.VK", "Failed to upload the photo to the server." + uploadPhotoData.FirstError.Description);

        var photoWall = await SendAsync($"photos.saveWallPhoto?access_token=TOKEN&group_id={_vkConfig.GroupId}&v={_vkConfig.ApiVersion}&photo={uploadPhotoData.Value.Photo}&server={uploadPhotoData.Value.ServerId}&hash={uploadPhotoData.Value.Hash}");
        var photoWallData = await photoWall.GetData<List<PhotoSaveWallPhotoResponse>>(true, false);

        if (photoWallData.IsError || photoWallData.Value?.Count == 0) return Error.Failure("Error.VK", "Failed to save the photo on the server." + photoWallData.FirstError.Description);

        return $"photo{photoWallData.Value![0].OwnerId}_{photoWallData.Value[0].Id}";
    }

    private async Task<ErrorOr<PhotoGetWallUploadServerResponse>> GetWallPhotoUploadServer()
    {
        var result = await SendAsync($"photos.getWallUploadServer?access_token=TOKEN&group_id={_vkConfig.GroupId}&v={_vkConfig.ApiVersion}");

        return await result.GetData<PhotoGetWallUploadServerResponse>(true, true);
    }

    internal async Task<HttpResponseMessage> SendAsync(string uri, HttpContent? content = null)
    {
        var token = await _vkAuthService.GetToken();
        
        if (!token.IsError && !string.IsNullOrWhiteSpace(token.Value))
        {
            if (content is null)
                return await _httpClient.GetAsync(AccessTokenRegex().Replace(uri, token.Value));
            else
                return await _httpClient.PostAsync(uri, content);
        }
        
        return default!;
    }

    [GeneratedRegex(@"(?<=access_token=)[^&]*")]
    private static partial Regex AccessTokenRegex();
}
