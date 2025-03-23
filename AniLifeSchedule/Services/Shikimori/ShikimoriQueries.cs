namespace AniLifeSchedule.Services.Shikimori;

public static class ShikimoriQueries
{
    public static readonly string ANIME_BY_TITLE =
@"query ($search: String) { 
    animes(search: $search, limit: 1) {
        id

        name
        russian

        kind
        status

        episodes
        episodesAired,
        nextEpisodeAt,

        poster { id originalUrl mainUrl }
      }
    }
}";

    public static readonly string ANIME_BY_IDS =
@"query ($id: String) { 
    animes(ids: $id) {
        id

        name
        russian

        kind
        status

        episodes
        episodesAired,
        nextEpisodeAt,

        poster { id originalUrl mainUrl }
      }
    }
}";
}
