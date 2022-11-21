using System.Text.Json;

namespace AniLifeSchedule.Services.Implementations;

public class ShikimoriService : IShikimoriService
{
    private readonly IHttpClientFactory _httpClientFactory = default!;
    private readonly HttpClient _httpClient = default!;

    public ShikimoriService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        _httpClient = _httpClientFactory.CreateClient("Shikimori");
    }

    public async Task<List<Anime>> GetAnime(int id)
    {
        string data = await _httpClient.GetStringAsync($"https://shikimori.one/api/animes?ids={id}");

        return JsonSerializer.Deserialize<List<Anime>>(data);
    }

    public async Task<List<Anime>> GetAnime(string title)
    {
        string data = await _httpClient.GetStringAsync($"https://shikimori.one/api/animes?search={title}");

        return JsonSerializer.Deserialize<List<Anime>>(data);
    }

    public Status GetStatusEnum(string value)
    {
        return value switch
        {
            "anons" => Status.NOT_YET_RELEASED,
            "ongoing" => Status.RELEASING,
            "released" => Status.FINISHED,
            _ => Status.RELEASING
        };
    }

    public Format GetFormatEnum(string value)
    {
        return value switch
        {
            "tv" => Format.TV,
            "movie" => Format.MOVIE,
            _ => Format.TV,
        };
    }
}
