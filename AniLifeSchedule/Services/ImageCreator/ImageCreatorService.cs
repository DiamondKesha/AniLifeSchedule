using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Models.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SkiaSharp;
using Topten.RichTextKit;

namespace AniLifeSchedule.Services.ImageCreator;

public class ImageCreatorService(
    IOptions<ScheduleConfiguration> scheduleConfiguration,
    IHttpClientFactory httpClientFactory,
    NavigationManager hostEnv) : IImageCreatorService
{
    private readonly NavigationManager _hostEnv = hostEnv;
    private readonly ScheduleConfiguration _scheduleConfiguration = scheduleConfiguration.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Base");

    public async Task<string> CreateScheduleImage(List<ScheduleModel> scheduleModels)
    {
        if (scheduleModels.Count == 0) return string.Empty;

        int height = (322 * (int)Math.Ceiling((decimal)scheduleModels.Count / 6) + 205);
        int width = 1280;

        using ImageCreator imageCreator = new(width, height);

        imageCreator.ChangeColorPaint("#18191B");
        imageCreator.DrawSKRect(new SKRect(0, 0, width, height));

        imageCreator.ResizeAndDrawImage(await GetImageFromUrl($"{_hostEnv.BaseUri}img/telegram.png"), 130, 40, new SKPoint((width / 2) - 100, height - 60)); // new SKPoint(1017, height - 60) 65
        imageCreator.ResizeAndDrawImage(await GetImageFromUrl($"{_hostEnv.BaseUri}img/vk.png"), 130, 40, new SKPoint((width / 2) + 35, height - 60)); // new SKPoint(873, height - 60)

        imageCreator.DrawText($"{scheduleModels[0].EpisodeReleaseDate.Day} {GetMonthGenitive(scheduleModels[0].EpisodeReleaseDate.Month)} покажут:", ImageStyles.ScheduleTitle, TextAlignment.Left, new SKPoint(137, 74), width, 1);

        int tempIndex = 0;
        float horizontalCoverPosition = 140, verticalCoverPosition = 130;

        for (int y = 0; y < Math.Ceiling((decimal)scheduleModels.Count / 6) + 1; y++)
        {
            for (int i = tempIndex; i < tempIndex + 6; i++)
            {
                if (i >= scheduleModels.Count) break;

                imageCreator.ResizeAndDrawImage(await GetImageFromUrl(scheduleModels[i].ImageUrl), 153, 230, new SKPoint(horizontalCoverPosition, verticalCoverPosition));

                var drawedTextResult = imageCreator.DrawText(GetTitle(scheduleModels[i].Titles.Russian, scheduleModels[i].Titles.Romaji), ImageStyles.ScheduleAnimeTitle,
                    TextAlignment.Left, new SKPoint(horizontalCoverPosition, verticalCoverPosition + 234.5f), 155, 3);

                imageCreator.DrawText($"{GetEpisodesOrLength(scheduleModels[i].Format, scheduleModels[i].CurrentAiringEpisode, scheduleModels[i].Episodes)} - {scheduleModels[i].EpisodeReleaseDate.ToShortTimeString()}", ImageStyles.ScheduleInfo,
                    TextAlignment.Left, new SKPoint(horizontalCoverPosition, verticalCoverPosition + 234.5f + drawedTextResult.Height), 153, 1);

                if (scheduleModels[i].Status.Equals(StatusType.NOT_YET_RELEASED))
                {
                    imageCreator.ChangeColorPaint("#029F52");
                    imageCreator.DrawSKRect(new SKRect(horizontalCoverPosition + 86, verticalCoverPosition + 20f, horizontalCoverPosition + 149, verticalCoverPosition + 4));
                    imageCreator.DrawText("ОНГОИНГ", ImageStyles.ScheduleAnimeStatus, TextAlignment.Left, new SKPoint(horizontalCoverPosition + 88f, verticalCoverPosition + 3), 80);
                }
                else if (scheduleModels[i].Status.Equals(StatusType.FINISHED))
                {
                    imageCreator.ChangeColorPaint("#CD0038");
                    imageCreator.DrawSKRect(new SKRect(horizontalCoverPosition + 100, verticalCoverPosition + 20f, horizontalCoverPosition + 149, verticalCoverPosition + 4));
                    imageCreator.DrawText("РЕЛИЗ", ImageStyles.ScheduleAnimeStatus, TextAlignment.Left, new SKPoint(horizontalCoverPosition + 103.3f, verticalCoverPosition + 3), 80);
                }
                else if (scheduleModels[i].Status.Equals(StatusType.PREVIEW))
                {
                    imageCreator.ChangeColorPaint("#225BA8");
                    imageCreator.DrawSKRect(new SKRect(horizontalCoverPosition + 68.5f, verticalCoverPosition + 20f, horizontalCoverPosition + 149, verticalCoverPosition + 4));
                    imageCreator.DrawText("ПРЕДПОКАЗ", ImageStyles.ScheduleAnimeStatus, TextAlignment.Left, new SKPoint(horizontalCoverPosition + 71f, verticalCoverPosition + 3), 80);
                }

                horizontalCoverPosition += 170;
            }

            horizontalCoverPosition = 140;
            verticalCoverPosition += 325;
            tempIndex += 6;
        }

        return imageCreator.Save(_scheduleConfiguration.PathToSave, _scheduleConfiguration.Filename.Replace("{date}", scheduleModels[0].EpisodeReleaseDate.ToShortDateString()));
    }

    #region Helpful methods

    private async Task<SKImage> GetImageFromUrl(string url)
    {
        var data = await _httpClient.GetStreamAsync(url);

        if (data != null) return SKImage.FromEncodedData(data);

        return SKImage.FromEncodedData(await _httpClient.GetStreamAsync("img/placeholder.png"));
    }

    private static string GetTitle(string russian, string romaji)
    {
        return !string.IsNullOrEmpty(russian) ? russian : romaji;
    }

    private static string GetEpisodesOrLength(FormatType format, string? currentEpisode, int? episodes, string length = default!, bool isShortEpisode = true)
    {
        if (format == FormatType.MOVIE && isShortEpisode) return "ФИЛЬМ";

        if (isShortEpisode)
        {
            return $"{currentEpisode}/{episodes?.ToString() ?? "?"}";
        }
        else
        {
            if (format == FormatType.MOVIE) return $"{length} мин.";

            return $"{episodes?.ToString() ?? "?"}";
        }
    }

    static string GetMonthGenitive(int month)
    {
        return month switch
        {
            1 => "января",
            2 => "февраля",
            3 => "марта",
            4 => "апреля",
            5 => "мая",
            6 => "июня",
            7 => "июля",
            8 => "августа",
            9 => "сентября",
            10 => "октября",
            11 => "ноября",
            12 => "декабря",
            _ => ""
        };
    }

    #endregion
}
