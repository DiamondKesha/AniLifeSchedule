using AniLifeSchedule.Enums;
using AniLifeSchedule.Models;
using AniLifeSchedule.Models.Shikimori;
using AniLifeSchedule.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

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

        private readonly NavigationManager _hostEnv;
        private readonly IAnilistService _anilistService;
        private readonly IImageCreatorService _imageService;
        private readonly IShikimoriService _shikimoriService;

        public ScheduleViewModel(NavigationManager hostEnv, 
                                 IImageCreatorService imageService,
                                 IAnilistService anilistService,
                                 IShikimoriService shikimoriService)
        {
            _hostEnv = hostEnv;
            _imageService = imageService;
            _anilistService = anilistService;
            _shikimoriService = shikimoriService;

            // For example
            Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = 1, Episodes = 12, ReleaseDate = DateTime.Today, ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
            Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = 1, Episodes = 12, ReleaseDate = DateTime.Today, ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
            Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = 1, Episodes = 12, ReleaseDate = DateTime.Today, ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
        }

        public async Task GetSchedule()
        {
            Models = new();

            var result = await _anilistService.GetScheduleByDate(DateInput);
            if (result?.Page?.AiringSchedules == null) return;

            foreach (var item in result.Page.AiringSchedules)
            {
                Models.Add(new ScheduleModel
                {
                    TitleRussian = "",
                    TitleRomaji = item.Media.Title.Romaji,
                    CurrentEpisode = item.Media.NextAiringEpisode.Episode == null ? 0 : (int)item.Media.NextAiringEpisode.Episode,
                    Episodes = item.Media.Episodes,
                    ReleaseDate = item.Media.NextAiringEpisode.GetTime,
                    ImageUrl = item.Media.Cover.Large,
                    Format = (Format)Enum.Parse(typeof(Format), item.Media.Format),
                    Status = (Status)Enum.Parse(typeof(Status), item.Media.Status)
                });
            }
        }

        public async Task AddItem()
        {
            if (!string.IsNullOrEmpty(ShikimoriInput))
            {
                List<Anime> anime;

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
                    Models.Add(new ScheduleModel
                    {
                        TitleRussian = anime[0].Title,
                        TitleRomaji = anime[0].TitleRomaji,
                        CurrentEpisode = anime[0].Episode == null ? 0 : (int)anime[0].Episode,
                        Episodes = anime[0].Episodes,
                        ReleaseDate = DateTime.Now,
                        ImageUrl = anime[0].Image.Original,
                        Format = _shikimoriService.GetFormatEnum(anime[0].Format),
                        Status = _shikimoriService.GetStatusEnum(anime[0].Status)
                    });

                    return;
                }
            }

            Models.Add(new ScheduleModel { TitleRussian = "Какое-то название", TitleRomaji = "Some Title", CurrentEpisode = 1, Episodes = 12, ReleaseDate = DateTime.Today.AddDays(1), ImageUrl = $"{_hostEnv.BaseUri}/img/placeholder.jpg", Format = Format.TV, Status = Status.FINISHED });
        }

        public async Task GetRussianTitle(int id)
        {
            var result = await _shikimoriService.GetAnime(Models[id].TitleRomaji);

            if (string.IsNullOrWhiteSpace(result?[0].Title)) return;

            Models[id].TitleRussian = result[0].Title;
        }

        public async Task Generate()
        {
            OutputImageData = await _imageService.Create(Models.ToList());
        }

        public void Sort()
        {
            Models = new ObservableCollection<ScheduleModel>(Models.OrderBy(x => x.ReleaseDate));
        }
    }
}
