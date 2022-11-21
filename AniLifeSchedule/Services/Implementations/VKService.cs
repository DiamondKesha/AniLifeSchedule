using AniLifeSchedule.Models.VK;
using AniLifeSchedule.Models.VK.Group;
using AniLifeSchedule.Models.VK.SaveFile;
using AniLifeSchedule.Models.VK.UploadFile;
using AniLifeSchedule.Models.VK.WallPost;
using AniLifeSchedule.Models.VK.Wrappers;
using AniLifeSchedule.Models.Wrapper;
using Microsoft.Extensions.Options;

namespace AniLifeSchedule.Services.Implementations;

public class VKService : IVKService
{
    private readonly IHttpClientFactory _httpClientFactory = default!;
    private readonly VKConfiguration _vkConfig = default!;

    private readonly HttpClient httpClient = default!;

    public VKService(IHttpClientFactory httpClientFactory,
                     IOptions<VKConfiguration> vkConfig)
    {
        _httpClientFactory = httpClientFactory;
        _vkConfig = vkConfig.Value;
        
        httpClient = _httpClientFactory.CreateClient("VK");
    }

    public async Task<Result<WallPost>> CreatePost(string token, string text, string attachments, DateTime? time = null)
    {
        string urlApi = $"wall.post?access_token={token}&owner_id=-{_vkConfig.GroupId}&message={text}&attachments={attachments}&v={_vkConfig.ApiVersion}";

        if (time != null && time >= DateTime.Now) urlApi += $"&publish_date={new DateTimeOffset(time.Value).ToUnixTimeSeconds()}";

        var result = await httpClient.GetAsync(urlApi);

        return await GetData<WallPost>(result, true, true);
    }

    public async Task<Result<List<GroupData>>> GetGroupInformation(string token)
    {
        var result = await httpClient.GetAsync($"groups.getById?access_token={token}&fields=name,members_count&group_id={_vkConfig.GroupId}&v={_vkConfig.ApiVersion}");

        return await GetData<List<GroupData>>(result, true, true);
    }

    public async Task<Result<UploadFileUrl>> GetWallUploadServer(string token)
    {
        var result = await httpClient.GetAsync($"docs.getWallUploadServer?access_token={token}&group_id={_vkConfig.GroupId}&v={_vkConfig.ApiVersion}");

        return await GetData<UploadFileUrl>(result, true, true);
    }

    public async Task<Result<UploadFileData>> UploadFileToServer(string uploadUrl, byte[] fileBytes, string filename)
    {
        MultipartFormDataContent form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(fileBytes), "file", filename);
        var result = await httpClient.PostAsync(uploadUrl, form);

        var data = await GetData<UploadFileData>(result, false, false);

        if (string.IsNullOrEmpty(data.Data.Error)) return data;

        return Result<UploadFileData>.Fail(default!, new List<string> { data.Data.Error, data.Data.ErrorDescription });
    }

    public async Task<Result<SaveFile>> SaveFileOnServer(string token, string fileString, string title, string tags)
    {
        var result = await httpClient.GetAsync($"docs.save?access_token={token}&file={fileString}&title={title}&tags={tags}&v={_vkConfig.ApiVersion}");

        return await GetData<SaveFile>(result, true, true);
    }

    public async Task<Result<int>> DeleteDocs(string token, string docId)
    {
        var result = await httpClient.GetAsync($"docs.delete?access_token={token}&owner_id=-{_vkConfig.GroupId}&doc_id={docId}&v={_vkConfig.ApiVersion}");

        var data = await GetData<object>(result, true, true);

        if (data.Succeeded) return Result<int>.Success(1);

        return Result<int>.Fail(0, data.Messages);
    }

    private async Task<Result<T>> GetData<T>(HttpResponseMessage? response, bool isResponseWrapper, bool isErrorWrapper) where T : class
    {
        if (response is null) return Result<T>.Fail(default!, "Request is failed. Response is null.");

        //string jsonString = await response.Content.ReadAsStringAsync();

        if (isResponseWrapper)
        {
            var jsonResult = await response.Content.ReadFromJsonAsync<ResponseWrapper<T>>();
            if (jsonResult?.Response is not null) return Result<T>.Success(jsonResult.Response);
        }
        else
        {
            var jsonResult = await response.Content.ReadFromJsonAsync<T>();
            if (jsonResult is not null) return Result<T>.Success(jsonResult);
        }

        if (isErrorWrapper)
        {
            var jsonError = await response.Content.ReadFromJsonAsync<ErrorWrapper<ErrorData>>();
            if (jsonError?.Error is not null) return Result<T>.Fail(default!, new List<string> { jsonError.Error.Id.ToString(), jsonError.Error.Message });
        }
        else
        {
            var jsonError = await response.Content.ReadFromJsonAsync<ErrorData>();
            if (jsonError is not null) return Result<T>.Fail(default!, new List<string> { jsonError.Id.ToString(), jsonError.Message });
        }

        return Result<T>.Fail(default!, "Data is null. Can't get result and error object.");
    }
}
