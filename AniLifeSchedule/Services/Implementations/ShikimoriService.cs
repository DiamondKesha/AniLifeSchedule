using AniLifeSchedule.Enums;
using AniLifeSchedule.Models.Shikimori;
using System.Text.Json;

namespace AniLifeSchedule.Services.Implementations
{
    public class ShikimoriService : IShikimoriService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient httpClient;

        public ShikimoriService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            httpClient = _httpClientFactory.CreateClient("Shikimori");
        }

        public async Task<List<Anime>> GetAnime(int id)
        {
            string data = await httpClient.GetStringAsync($"https://shikimori.one/api/animes?ids={id}");

            return JsonSerializer.Deserialize<List<Anime>>(data);
        }

        public async Task<List<Anime>> GetAnime(string title)
        {
            string data = await httpClient.GetStringAsync($"https://shikimori.one/api/animes?search={title}");

            return JsonSerializer.Deserialize<List<Anime>>(data);
        }

        public Status GetStatusEnum(string value)
        {
            switch (value)
            {
                case "anons":
                    return Status.NOT_YET_RELEASED;
                    break;
                case "ongoing":
                    return Status.RELEASING;
                    break;
                case "released":
                    return Status.FINISHED;
                    break;
                default:
                    return Status.RELEASING;
                    break;
            }
        }

        public Format GetFormatEnum(string value)
        {
            switch (value)
            {
                case "tv":
                    return Format.TV;
                    break;
                case "movie":
                    return Format.MOVIE;
                    break;
                default:
                    return Format.TV;
                    break;
            }
        }
    }
}
