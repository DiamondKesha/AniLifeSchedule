using AniLifeSchedule.Data;
using AniLifeSchedule.Enums;
using AniLifeSchedule.Models;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Models.Shikimori;
using AniLifeSchedule.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace AniLifeSchedule.ViewModels.Implementations
{
    public class ScheduleViewModel : IScheduleViewModel
    {
        private DateTime _dateInput = DateTime.Today.AddDays(1);
        public DateTime DateInput { get => _dateInput; set => _dateInput = value; }

        private string _shikimoriInput;
        public string ShikimoriInput { get => _shikimoriInput; set => _shikimoriInput = value; }

        private ObservableCollection<ScheduleModel> models = new ObservableCollection<ScheduleModel>();
        public ObservableCollection<ScheduleModel> Models { get => models; set => models = value; }

        private string _outputImageData;
        public string OutputImageData { get => _outputImageData; set => _outputImageData = value; }

        private readonly ScheduleConfiguration _scheduleConfig;
        private readonly ScheduleTransferData _scheduleData;
        private readonly NavigationManager _hostEnv;
        private readonly IAnilistService _anilistService;
        private readonly IImageCreatorService _imageService;
        private readonly IShikimoriService _shikimoriService;
        private readonly IVKService _vkService;
        private readonly ICookiesService _cookiesService;
        private readonly IToastService _toastService;

        public ScheduleViewModel(IOptions<ScheduleConfiguration> scheduleConfig,
                                 ScheduleTransferData scheduleData,
                                 NavigationManager hostEnv,
                                 IImageCreatorService imageService,
                                 IAnilistService anilistService,
                                 IShikimoriService shikimoriService,
                                 IVKService vkService,
                                 ICookiesService cookiesService,
                                 IToastService toastService
            )
        {
            _hostEnv = hostEnv;
            _imageService = imageService;
            _anilistService = anilistService;
            _shikimoriService = shikimoriService;
            _scheduleData = scheduleData;
            _vkService = vkService;
            _cookiesService = cookiesService;
            _scheduleConfig = scheduleConfig.Value;
            _toastService = toastService;

            OnStartMethod();
        }

        #region Main Buttons 

        public async Task GetSchedule()
        {
            Models = new();
            _scheduleData.Schedule = new();

            _toastService.ShowInfo("Try to get schedule from AniList...");

            var result = await _anilistService.GetScheduleByDate(DateInput);
            if (result?.Page?.AiringSchedules == null)
            {
                _toastService.ShowError("Airing schedule is null.");
                return;
            }

            foreach (var item in result.Page.AiringSchedules)
            {
                Format tempFormat = (Format)Enum.Parse(typeof(Format), item.Media.Format);

                Models.Add(new ScheduleModel
                {
                    TitleRussian = "",
                    TitleRomaji = item.Media.Title.Romaji,
                    CurrentEpisode = item.Media.NextAiringEpisode.Episode == null ? "0" : item.Media.NextAiringEpisode.Episode.ToString(),
                    Episodes = item.Media.Episodes,
                    ReleaseDate = item.Media.NextAiringEpisode.GetTime,
                    ImageUrl = item.Media.Cover.Large,
                    Format = tempFormat,
                    Status = tempFormat == Format.MOVIE ? Status.FINISHED : (Status)Enum.Parse(typeof(Status), item.Media.Status)
                });
            }

            _scheduleData.Schedule = Models.ToList();
            _toastService.ShowSuccess("Airing schedule is successfully getted.");
        }

        public async Task AddItem()
        {
            ScheduleModel model = new();

            if (!string.IsNullOrEmpty(ShikimoriInput))
            {
                List<Anime> anime = new();

                if (int.TryParse(ShikimoriInput, out int result))
                {
                    anime = await _shikimoriService.GetAnime(result);
                }
                else
                {
                    anime = await _shikimoriService.GetAnime(ShikimoriInput);
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
                        Format = _shikimoriService.GetFormatEnum(anime[0].Format),
                        Status = _shikimoriService.GetStatusEnum(anime[0].Status)
                    };

                    Models.Add(model);
                    _scheduleData.Schedule.Add(model);

                    return;
                }
            }

            model = new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today.AddDays(1), ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED };

            Models.Add(model);
            _scheduleData.Schedule.Add(model);
        }

        public async Task GetRussianTitle(int id)
        {
            var result = await _shikimoriService.GetAnime(Models[id].TitleRomaji);

            if (string.IsNullOrWhiteSpace(result?[0].Title)) return;

            Models[id].TitleRussian = result[0].Title;
            _scheduleData.Schedule[id].TitleRussian = result[0].Title;
        }

        public async Task Generate()
        {
            _toastService.ShowInfo($"Generating image with {Models.Count} titles has started...");

            OutputImageData = await _imageService.Create(Models.ToList());

            _toastService.ShowSuccess("Generating image is completed...");
        }

        public void Sort()
        {
            Models = new ObservableCollection<ScheduleModel>(Models.OrderBy(x => x.ReleaseDate));
        }

        #endregion

        #region Schedule Callbacks

        public void SaveCallback(int id)
        {
            _scheduleData.Schedule[id] = Models[id];
        }

        public void RemoveCallback(int id)
        {
            _scheduleData.Schedule.RemoveAt(id);
        }

        #endregion

        public async Task UploadFile()
        {
            Regex regex = new(@"doc-\w+\d+?");
            string filename = _scheduleConfig.Filename.Replace("{date}", DateInput.ToShortDateString());
            string filePath = $"{_scheduleConfig.PathToSave}\\{filename}.jpg";

            _toastService.ShowInfo("Try to upload and post image to group VK...");

            if (!File.Exists(filePath))
            {
                _toastService.ShowError($"Image file didn't exists! Try to generate and then try again.\n{filePath}");
                return;
            }

            var upload = await _vkService.GetWallUploadServer(_cookiesService.AccessToken);

            if (upload.Succeeded)
            {
                var uploadFile = await _vkService.UploadFileToServer(upload.Data.Url, await File.ReadAllBytesAsync(filePath), $"{filename}.jpg");

                if (uploadFile.Succeeded)
                {
                    var saveFile = await _vkService.SaveFileOnServer(_cookiesService.AccessToken, uploadFile.Data.File, filename, _scheduleConfig.FileTags);

                    if (saveFile.Succeeded)
                    {
                        var post = await _vkService.CreatePost(_cookiesService.AccessToken, _scheduleConfig.Message.Replace("{date}", DateInput.ToShortDateString()), regex.Match(saveFile.Data.Document.Url).Value, DateTime.Today + TimeSpan.Parse(_scheduleConfig.TimeToPost));

                        if (post.Succeeded)
                        {
                            _toastService.ShowSuccess($"Image uploading and post creating is completed. Post id: {post.Data.PostId}");
                            return;
                        }

                        _toastService.ShowError(post.Messages[0]);
                    }
                    else
                    {
                        _toastService.ShowError(saveFile.Messages[0]);
                        return;
                    }
                }
                else
                {
                    _toastService.ShowError(uploadFile.Messages[0]);
                    return;
                }
            }
            else
            {
                _toastService.ShowError(upload.Messages[0]);
                return;
            }
        }

        private void OnStartMethod()
        {
            if (_scheduleData.Schedule == null || _scheduleData.Schedule?.Count == 0)
            {
                Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today, ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
                Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today, ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
                Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = "1", Episodes = 12, ReleaseDate = DateTime.Today, ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });

                _scheduleData.Schedule = Models.ToList();
            }
            else
            {
                for (int i = 0; i < _scheduleData.Schedule.Count; i++)
                {
                    Models.Add(_scheduleData.Schedule[i]);
                }
            }
        }
    }
}
