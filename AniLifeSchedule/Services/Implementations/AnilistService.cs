using AniLifeSchedule.Models.Anilist;
using GraphQL;
using GraphQL.Client.Http;
using System.Text.Json;

namespace AniLifeSchedule.Services.Implementations
{
    public class AnilistService : IAnilistService
    {
        private readonly IHttpClientFactory _httpClient;

        private readonly string _scheduleQuery =
        @"query ($id: Int, $startDay: Int, $endDay: Int) { 
                    Page(page: 1) {
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
                                isAdult } } } }";

        public AnilistService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AnilistData?> GetScheduleByDate(DateTime date)
        {
            var data = await SendGraphQuery(_scheduleQuery, new
            {
                startDay = ((DateTimeOffset)date.Date).ToUnixTimeSeconds(),
                endDay = ((DateTimeOffset)date.Date.AddDays(1).AddTicks(-1)).ToUnixTimeSeconds()
            });

            return JsonSerializer.Deserialize<AnilistData>(data.ToString());
        }

        private async Task<object?> SendGraphQuery( string query, object? variables)
        {
            var httpClient = _httpClient.CreateClient("Anilist");

            GraphQLHttpClient graphQLClient = new GraphQLHttpClient(new GraphQLHttpClientOptions(),
                new GraphQL.Client.Serializer.SystemTextJson.SystemTextJsonSerializer(), httpClient);

            GraphQLRequest request = new GraphQLRequest
            {
                Query = query,
                Variables = variables
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<object>(request);
            return graphQLResponse.Data;
        }
    }
}
