using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Enums;
using AniLifeSchedule.Models.Shikimori;
using AniLifeSchedule.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using AniLifeSchedule.Models.Configurations;
using Microsoft.Extensions.Options;
using Blazored.LocalStorage;
using System.Text.Json;
using AniLifeSchedule.Common.Base;

namespace AniLifeSchedule.Components.Pages;

public partial class Home
{

    [GeneratedRegex(@"^-?[0-9]+(?:\.[0-9]+)?$")]
    private static partial Regex IsDigitRegex();

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
    [Inject] protected IAnilistService AnilistService { get; set; } = default!;
    [Inject] protected IShikimoriService ShikimoriService { get; set; } = default!;
    [Inject] protected IVKService VKService { get; set; } = default!;
    [Inject] protected IImageCreatorService ImageCreatorService { get; set; } = default!;
    [Inject] protected ILocalStorageService LocalStorageService { get; set; } = default!;
    [Inject] protected IOptions<ScheduleConfiguration> ScheduleConfiguration { get; set; } = default!;

    private ScheduleConfiguration _scheduleConfiguration = default!;

    private List<ScheduleModel> _scheduleModels { get; set; } = [];
    private DateTime? _dateInput { get; set; } = DateTime.Today.AddDays(1);
    private string _generatedImage { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool onFirstRenderer)
    {
        if (onFirstRenderer)
        {
            _scheduleConfiguration = ScheduleConfiguration.Value;
        }

        string? json = await LocalStorageService.GetItemAsStringAsync(Constants.LOCALSTORAGE_SCHEDULE_DATA);
        if (string.IsNullOrWhiteSpace(json)) return;

        _scheduleModels = JsonSerializer.Deserialize<List<ScheduleModel>>(json) ?? [];
        StateHasChanged();
    }

    #region Buttons 

    public async Task GetSchedule()
    {
        int page = 1;
        bool isHaveNextPage = true;

        if (_dateInput is null || DateTime.UtcNow.AddDays(-1) > _dateInput)
        {
            SnackbarService.Add("Please, set a valid date", Severity.Warning);
            return;
        }

        _scheduleModels.Clear();
        StateHasChanged();

        await UpdateLocalStorageScheduleData();

        while (isHaveNextPage)
        {
            var result = await AnilistService.GetScheduleByDate((DateTime)_dateInput, page);

            if (result.IsError)
            {
                SnackbarService.Add(result.FirstError.Description, Severity.Error);
                return;
            }

            foreach (var item in result.Value.Page.AiringSchedule!)
            {
                FormatType tempFormat = (FormatType)Enum.Parse(typeof(FormatType), item.Media.Format);
                string? tempCurrentEpisode = item.Media.NextAiringEpisode?.Episode == null ? "0" : item.Media.NextAiringEpisode.Episode.ToString();

                _scheduleModels.Add(ScheduleModel.Create(
                    TitleModel.Create("", item.Media.Title.Romaji),
                    item.Media.Cover.ExtraLarge ?? item.Media.Cover.Large,
                    tempCurrentEpisode!,
                    item.Media.NextAiringEpisode?.GetTime ?? default,
                    item.Media.Episodes,
                    tempFormat,
                    GetStatus(tempFormat, item.Media.Status, int.Parse(tempCurrentEpisode!), item.Media.Episodes))
                );
            }

            isHaveNextPage = result.Value.Page.PageInfo.HasNextPage;
            page++;
        }

        await UpdateLocalStorageScheduleData();
    }

    public async Task GetFromShikimori()
    {
        var dialog = await DialogService.ShowAsync<ShikimoriSearchDialog>();
        var dialogResult = await dialog.Result;

        if (dialogResult is null || dialogResult.Canceled) return;

        string result = (string)dialogResult.Data!;

        ErrorOr<ShikimoriResponse> shikimoriResponse;

        if (result.Split(",").All(IsDigitRegex().IsMatch))
        {
            shikimoriResponse = await ShikimoriService.GetAnimeByIds(result);
        }
        else
        {
            shikimoriResponse = await ShikimoriService.GetAnimeByTitle(result);
        }

        if (shikimoriResponse.IsError)
        {
            SnackbarService.Add(shikimoriResponse.FirstError.Description, Severity.Error);
            return;
        }

        foreach (var item in shikimoriResponse.Value.Animes)
        {
            _scheduleModels.Add(ScheduleModel.Create(
                TitleModel.Create(item.Russian, item.Romaji),
                item.Poster.OriginalUrl,
                item.EpisodesAired is not null ? ((int)item.EpisodesAired).ToString() : "?",
                item.NextEpisodeReleaseDate ?? DateTime.Today.AddDays(1),
                item.Episodes,
                item.GetFormatEnum(),
                item.GetStatusEnum()));
        }

        await UpdateLocalStorageScheduleData();
    }

    public async Task GenerateImage()
    {
        _generatedImage = await ImageCreatorService.CreateScheduleImage(_scheduleModels);
    }

    public async Task CreatePost()
    {
        var uploadPhotoReuslt = await VKService.UploadPhotoToServer($"{_scheduleConfiguration.PathToSave}\\{_scheduleConfiguration.Filename.Replace("{date}", _scheduleModels.First().EpisodeReleaseDate.ToShortDateString())}.jpg");

        if (uploadPhotoReuslt.IsError)
        {
            SnackbarService.Add(uploadPhotoReuslt.Errors.First().Description, Severity.Error);
            return;
        }

        var postCreationResult = await VKService.CreatePost(
            _scheduleConfiguration.Message.Replace("{date}", _dateInput?.ToShortDateString()),
            uploadPhotoReuslt.Value,
            _dateInput?.AddDays(-1) + TimeSpan.Parse(_scheduleConfiguration.TimeToPost));

        if(postCreationResult.IsError)
        {
            SnackbarService.Add(postCreationResult.Errors.First().Description, Severity.Error);
            return;
        }

        SnackbarService.Add("Post created. Id: " + postCreationResult.Value.PostId, Severity.Normal);
    }

    public async Task AddCard()
    {
        var dialog = await DialogService.ShowAsync<EditScheduleCardDialog>("");
        var dialogResult = await dialog.Result;

        if (dialogResult is null || dialogResult.Canceled || dialogResult.Data is not ScheduleModel)
        {
            return;
        }

        _scheduleModels.Add((ScheduleModel)dialogResult.Data);

        await UpdateLocalStorageScheduleData();
    }

    public async Task SortCards()
    {
        _scheduleModels = new List<ScheduleModel>(_scheduleModels.OrderBy(x => x.EpisodeReleaseDate));

        await UpdateLocalStorageScheduleData();
    }

    public async Task EditCard(int id)
    {
        if (_scheduleModels.Count < id) return;

        var parameters = new DialogParameters<EditScheduleCardDialog> {
                { x => x.ScheduleModel, _scheduleModels[id] }
        };

        var dialog = await DialogService.ShowAsync<EditScheduleCardDialog>("", parameters);
        var dialogResult = await dialog.Result;

        if (dialogResult is null || dialogResult.Canceled || dialogResult.Data is not ScheduleModel)
        {
            return;
        }

        _scheduleModels[id] = (ScheduleModel)dialogResult.Data;

        await UpdateLocalStorageScheduleData();
    }

    public async Task GetRussianTitle(int id)
    {
        if (_scheduleModels.Count < id) return;

        var result = await ShikimoriService.GetAnimeByTitle(_scheduleModels[id].Titles.Romaji);

        if (result.IsError)
        {
            SnackbarService.Add(result.FirstError.Description, Severity.Error);
            return;
        }

        _scheduleModels[id].Titles.Russian = result.Value.Animes[0].Russian;

        await UpdateLocalStorageScheduleData();
    }

    public async Task RemoveCard(int id)
    {
        if (_scheduleModels.Count < id) return;

        _scheduleModels.RemoveAt(id);

        await UpdateLocalStorageScheduleData();
    }

    #endregion

    private static StatusType GetStatus(FormatType format, string status, int currentEpisode, int? episodes)
    {
        if (format == FormatType.MOVIE || currentEpisode == episodes) return StatusType.FINISHED;

        bool isParse = Enum.TryParse(status, true, out StatusType currentStatus);

        return isParse ? currentStatus : StatusType.NOT_YET_RELEASED;
    }

    private async Task UpdateLocalStorageScheduleData()
    {
        string json = JsonSerializer.Serialize(_scheduleModels);

        await LocalStorageService.SetItemAsStringAsync(Constants.LOCALSTORAGE_SCHEDULE_DATA, json);
    }

}