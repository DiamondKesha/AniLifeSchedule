using AniLifeSchedule.Models.VK.Wrappers;
using AniLifeSchedule.Models.VK;
using System.Text.Json;

namespace AniLifeSchedule.Common.Extensions;

public static class VKServiceExtension
{
    public static async Task<ErrorOr<T>> GetData<T>(this HttpResponseMessage? response, bool isResponseWrapper, bool isErrorWrapper)
    {
        if (response is null) return Error.Failure("Error.VK", "Request is failed. Response is null.");

        var content = await response.Content.ReadAsStringAsync();
        JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

        if (isResponseWrapper)
        {
            var jsonResult = JsonSerializer.Deserialize<ResponseWrapper<T>>(content, jsonOptions);
            if (jsonResult!.Response is not null) return jsonResult.Response;
        }
        else
        {
            Console.WriteLine(content);
            var jsonResult = JsonSerializer.Deserialize<T>(content, jsonOptions);
            if (jsonResult is not null) return jsonResult;
        }

        if (isErrorWrapper)
        {

            var jsonError = JsonSerializer.Deserialize<ErrorWrapper<ErrorData>>(content, jsonOptions);
            if (jsonError?.Error is not null) return Error.Failure("Error.VK", $"№{jsonError.Error.Id}, {jsonError.Error.Message}");
        }
        else
        {
            var jsonError = JsonSerializer.Deserialize<ErrorData>(content, jsonOptions);
            if (jsonError is not null) return Error.Failure("Error.VK", $"№{jsonError.Id}, {jsonError.Message}");
        }

        return Error.Failure("Error.VK", "Data is null. Can't get result and error object.");
    }
}
