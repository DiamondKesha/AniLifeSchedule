using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Anilist;
using GraphQL;
using GraphQL.Client.Http;
using System.Text.Json;

namespace AniLifeSchedule.Services.Anilist;

public class AnilistService(IHttpClientFactory httpClientFactory) : IAnilistService
{
    private HttpClient _httpClient { get; set; } = httpClientFactory.CreateClient("Anilist");

    public async Task<ErrorOr<AnilistResponse<AnilistAiringScheduleResponse>>> GetScheduleByDate(DateTime date, int? page = null)
    {
        var data = await SendGraphQuery(AnilistQueries.SCHEDULE, new
        {
            page = page ?? 1,
            startDay = ((DateTimeOffset)date.Date).ToUnixTimeSeconds(),
            endDay = ((DateTimeOffset)date.Date.AddDays(1).AddTicks(-1)).ToUnixTimeSeconds()
        });

        if (data.IsError) return data.Errors;

        var responseData = JsonSerializer.Deserialize<AnilistResponse<AnilistAiringScheduleResponse>>(data.Value.ToString()!);

        if (responseData is null) return Error.Unexpected("Error.Anilist", "Response is null.");

        return responseData;
    }

    private async Task<ErrorOr<object>> SendGraphQuery(string query, object? variables)
    {
        GraphQLHttpClient graphQLClient = new(new GraphQLHttpClientOptions(),
            new GraphQL.Client.Serializer.SystemTextJson.SystemTextJsonSerializer(), _httpClient);

        GraphQLRequest request = new()
        {
            Query = query,
            Variables = variables
        };

        var graphQLResponse = await graphQLClient.SendQueryAsync<object>(request);

        if(graphQLResponse.Errors?.Length > 0)
        {
            List<Error> errors = [];

            foreach (var item in graphQLResponse.Errors)
            {
                errors.Add(Error.Unexpected("Error.Anilist", item.Message));
            }

            return errors;
        }

        return graphQLResponse.Data;
    }
}
