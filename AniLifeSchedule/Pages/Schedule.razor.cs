using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace AniLifeSchedule.Pages;

public partial class Schedule
{
    #region Injection

    [Inject] protected IOptions<ScheduleConfiguration> ScheduleConfiguration { get; set; } = default!;
    [Inject] protected ScheduleTransferData ScheduleTransferData { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IAnilistService AnilistService { get; set; } = default!;
    [Inject] protected IImageCreatorService ImageService { get; set; } = default!;
    [Inject] protected IShikimoriService ShikimoriService { get; set; } = default!;
    [Inject] protected IVKService VKService { get; set; } = default!;
    [Inject] protected ICookiesService CookiesService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;

    #endregion

    private ScheduleConfiguration _scheduleConfig { get; set; } = default!;

    private ObservableCollection<ScheduleModel> _data { get; set; } = new();
    private DateTime _dateInput { get; set; } = DateTime.Today.AddDays(1);
    private string _titleInput { get; set; } = string.Empty;
    private string _outputImageData { get; set; } = string.Empty;

    protected override Task OnInitializedAsync()
    {
        if (ScheduleTransferData.Schedule == null || ScheduleTransferData.Schedule?.Count == 0)
        {
            _data.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today.AddDays(1), ImageUrl = $"{NavigationManager.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
            _data.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today.AddDays(1), ImageUrl = $"{NavigationManager.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
            _data.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today.AddDays(1), ImageUrl = $"{NavigationManager.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });

            ScheduleTransferData.Schedule = _data.ToList();
        }
        else
        {
            for (int i = 0; i < ScheduleTransferData?.Schedule?.Count; i++)
            {
                _data.Add(ScheduleTransferData.Schedule[i]);
            }
        }

        _scheduleConfig = ScheduleConfiguration.Value;

        return base.OnInitializedAsync();
    }

    #region Button Handlers

    public async Task GetSchedule()
    {
        _data = new();
        ScheduleTransferData.Schedule = new();

        ToastService.ShowInfo("Try to get schedule from AniList...");

        var result = await AnilistService.GetScheduleByDate(_dateInput);
        if (result?.Page?.AiringSchedules == null)
        {
            ToastService.ShowError("Airing schedule is null.");
            return;
        }

        foreach (var item in result.Page.AiringSchedules)
        {
            Format tempFormat = (Format)Enum.Parse(typeof(Format), item.Media.Format);
            string tempCurrentEpisode = item.Media.NextAiringEpisode?.Episode == null ? "0" : item.Media.NextAiringEpisode.Episode.ToString();

            _data.Add(new ScheduleModel
            {
                TitleRussian = "",
                TitleRomaji = item.Media.Title.Romaji,
                CurrentEpisode = tempCurrentEpisode,
                Episodes = item.Media.Episodes,
                ReleaseDate = item.Media.NextAiringEpisode?.GetTime ?? default,
                ImageUrl = item.Media.Cover.Large,
                Format = tempFormat,
                Status = GetStatus(tempFormat, item.Media.Status, int.Parse(tempCurrentEpisode), item.Media.Episodes)
            });
        }

        ScheduleTransferData.Schedule = _data.ToList();
        ToastService.ShowSuccess("Airing schedule is successfully getted.");
    }

    public async Task AddItem()
    {
        ScheduleModel model = new();

        if (!string.IsNullOrEmpty(_titleInput))
        {
            List<Anime> anime = new();

            if (int.TryParse(_titleInput, out int result))
            {
                anime = await ShikimoriService.GetAnime(result);
            }
            else
            {
                anime = await ShikimoriService.GetAnime(_titleInput);
            }

            if (anime != null)
            {
                model = new ScheduleModel
                {
                    TitleRussian = anime[0].Title,
                    TitleRomaji = anime[0].TitleRomaji,
                    CurrentEpisode = anime[0].Episode == null ? "0" : anime[0].Episode.ToString(),
                    Episodes = anime[0].Episodes,
                    ReleaseDate = DateTime.Now,
                    ImageUrl = anime[0].Image.Original,
                    Format = ShikimoriService.GetFormatEnum(anime[0].Format),
                    Status = ShikimoriService.GetStatusEnum(anime[0].Status)
                };

                _data.Add(model);
                ScheduleTransferData.Schedule.Add(model);

                return;
            }
        }

        model = new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today.AddDays(1), ImageUrl = $"{NavigationManager.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED };

        _data.Add(model);
        ScheduleTransferData.Schedule.Add(model);
    }

    public async Task GetRussianTitle(int id)
    {
        var result = await ShikimoriService.GetAnime(_data[id].TitleRomaji);

        if (string.IsNullOrWhiteSpace(result?[0].Title)) return;

        foreach(var item in result)
        {

            // Most of the names on romaji are the same, so it’s okay
            if (item.TitleRomaji == _data[id].TitleRomaji)
            {
                _data[id].TitleRussian = result[0].Title;
                ScheduleTransferData.Schedule[id].TitleRussian = result[0].Title;

                return;
            }
        }

        // If titles is not equals return first title.
        _data[id].TitleRussian = result[0].Title;
        ScheduleTransferData.Schedule[id].TitleRussian = result[0].Title;
    }

    public async Task Generate()
    {
        ToastService.ShowInfo($"Generating image with {_data.Count} titles has started...");

        _outputImageData = await ImageService.Create(_data.ToList());

        ToastService.ShowSuccess("Generating image is completed...");
    }

    public void Sort() => _data = new ObservableCollection<ScheduleModel>(_data.OrderBy(x => x.ReleaseDate));

    #endregion

    #region Schedule Callbacks

    public void SaveCallback(int id)
    {
        ScheduleTransferData.Schedule[id] = _data[id];
    }

    public void RemoveCallback(int id)
    {
        ScheduleTransferData.Schedule.RemoveAt(id);
    }

    #endregion

    public async Task UploadFile()
    {
        Regex regex = new(@"doc-\w+\d+?");
        string filename = _scheduleConfig.Filename.Replace("{date}", _dateInput.ToShortDateString());
        string filePath = $"{_scheduleConfig.PathToSave}\\{filename}.jpg";

        ToastService.ShowInfo("Try to upload and post image to group VK...");

        if (!File.Exists(filePath))
        {
            ToastService.ShowError($"Image file didn't exists! Try to generate and then try again.\n{filePath}");
            return;
        }

        var upload = await VKService.GetWallUploadServer(CookiesService.AccessToken);
        if (!upload.Succeeded)
        {
            ToastService.ShowError(upload.Messages[0]);
            return;
        }

        var uploadFile = await VKService.UploadFileToServer(upload.Data.Url, await File.ReadAllBytesAsync(filePath), $"{filename}.jpg");
        if (!uploadFile.Succeeded)
        {
            ToastService.ShowError(uploadFile.Messages[0]);
            return;
        }

        var saveFile = await VKService.SaveFileOnServer(CookiesService.AccessToken, uploadFile.Data.File, filename, _scheduleConfig.FileTags);
        if (!saveFile.Succeeded)
        {
            ToastService.ShowError(saveFile.Messages[0]);
            return;
        }

        var post = await VKService.CreatePost(CookiesService.AccessToken, _scheduleConfig.Message.Replace("{date}", _dateInput.ToShortDateString()), regex.Match(saveFile.Data.Document.Url).Value, _dateInput.AddDays(-1) + TimeSpan.Parse(_scheduleConfig.TimeToPost));
        if (!post.Succeeded)
        {
            ToastService.ShowError(post.Messages[0]);
            await DeleteImage(saveFile.Data.Document.Id.ToString(), filePath);

            return;
        }

        ToastService.ShowSuccess($"Image uploading and post creating is completed. Post id: {post.Data.PostId}");
        await DeleteImage(saveFile.Data.Document.Id.ToString(), filePath);
    }

    private Status GetStatus(Format format, string status, int currentEpisode, int? episodes)
    {
        if (format == Format.MOVIE || currentEpisode == episodes) return Status.FINISHED;

        bool isParse = Enum.TryParse(status, true, out Status currentStatus);

        return isParse ? currentStatus : Status.NOT_YET_RELEASED;
    }

    private async Task DeleteImage(string docId = default!, string filePath = default!)
    {
        if (_scheduleConfig.DeleteVkDocumentAfterPost && !string.IsNullOrWhiteSpace(docId))
        {
            var result = await VKService.DeleteDocs(CookiesService.AccessToken, docId);

            if (result.Succeeded) ToastService.ShowSuccess("Document is deleted! " + result.Data);
            else ToastService.ShowSuccess("Document is not deleted! " + result.Data + result.Messages);
        }

        if(_scheduleConfig.DeleteFileAfterPost && File.Exists(filePath))
        {
            File.Delete(filePath);
            ToastService.ShowSuccess("File is deleted!");
        }
    }
}