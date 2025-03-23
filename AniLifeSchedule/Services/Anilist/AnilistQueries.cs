namespace AniLifeSchedule.Services.Anilist;

public static class AnilistQueries
{
    public static readonly string SCHEDULE =
@"query ($id: Int, $page: Int, $startDay: Int, $endDay: Int) { 
        Page(page:$page) {
            pageInfo {
                hasNextPage
            }
            airingSchedules(id: $id, sort: TIME, airingAt_lesser: $endDay, airingAt_greater: $startDay) {
                media {
                    nextAiringEpisode { airingAt episode }
                    title { romaji english native }
                    coverImage { large }
                    episodes
                    startDate { year month day }
                    status
                    format
                    averageScore
                    isAdult 
                } 
            } 
        } 
    }";

    public static readonly string AIRING_NEW_ANIME =
@"query ($id: Int, $page: Int, $startDate: FuzzyDateInt, $endDate: FuzzyDateInt) {
        Page(page:$page) {
            pageInfo{
                hasNextPage
            }
            media(id: $id, startDate_greater: $startDate, startDate_lesser: $endDate, type: ANIME) {
                nextAiringEpisode {
                    airingAt
                    episode
                }
                title {
                    romaji
                    english
                    native
                }
                coverImage {
                    large
                }
                episodes
                startDate {
                    year
                    month
                    day
                }
                status
                studios {
                    edges {
                        node {
                            name
                            isAnimationStudio
                        }
                }
                }
                duration
                format
                description
                averageScore
                isAdult
            }
        }
    }
    ";
}
