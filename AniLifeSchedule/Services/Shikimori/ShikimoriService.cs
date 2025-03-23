using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Shikimori;
using GraphQL;
using GraphQL.Client.Http;
using System.Text.Json;

namespace AniLifeSchedule.Services.Shikimori;

public class ShikimoriService(IHttpClientFactory httpClientFactory) : IShikimoriService
{

    private HttpClient _httpClient { get; set; } = httpClientFactory.CreateClient("Shikimori");

    public async Task<ErrorOr<ShikimoriResponse>> GetAnimeByIds(string ids)
    {
        var data = await SendGraphQuery(ShikimoriQueries.ANIME_BY_IDS, new
        {
            id = ids
        });

        if (data.IsError) return data.Errors;

        var responseData = JsonSerializer.Deserialize<ShikimoriResponse>(data.Value.ToString()!);

        if (responseData is null || responseData.Animes.Count == 0) return Error.Unexpected("Error.Shikimori", "Response is null.");

        return responseData;
    }

    public async Task<ErrorOr<ShikimoriResponse>> GetAnimeByTitle(string title)
    {
        var data = await SendGraphQuery(ShikimoriQueries.ANIME_BY_TITLE, new
        {
            search = title
        });

        if (data.IsError) return data.Errors;

        var responseData = JsonSerializer.Deserialize<ShikimoriResponse>(data.Value.ToString()!);

        if (responseData is null || responseData.Animes.Count == 0) return Error.Unexpected("Error.Shikimori", "Response is null.");

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

        if (graphQLResponse.Errors?.Length > 0)
        {
            List<Error> errors = [];

            foreach (var item in graphQLResponse.Errors)
            {
                errors.Add(Error.Unexpected("Error.Shikimori", item.Message));
            }

            return errors;
        }

        return graphQLResponse.Data;
    }
}