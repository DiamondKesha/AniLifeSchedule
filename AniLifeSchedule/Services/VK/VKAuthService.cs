using AniLifeSchedule.Common.Base;
using AniLifeSchedule.Common.Extensions;
using AniLifeSchedule.Contracts.Services;
using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Models.VK.OAuth;
using AniLifeSchedule.Models.VK.Wrappers;
using Blazored.LocalStorage;
using Microsoft.Extensions.Options;

namespace AniLifeSchedule.Services.VK;

public class VKAuthService(ILocalStorageService localStorage, IHttpClientFactory httpClientFactory, IOptions<VKConfiguration> vkConfig) : IVKAuthService
{
    private ILocalStorageService _localStorage { get; set; } = localStorage;

    private HttpClient _httpClient { get; set; } = httpClientFactory.CreateClient("VKOuath");

    private readonly VKConfiguration _vkConfig = vkConfig.Value;

    public async Task<ErrorOr<bool>> Authorize(string code, string deviceId)
    {
        string? codeVerifer = await _localStorage.GetItemAsStringAsync(Constants.LOCALSTORAGE_PKCE_VERIFIER);

        var formData = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "authorization_code"),
                new("code_verifier", codeVerifer ?? ""),
                new("redirect_uri",_vkConfig.RedirectUrl),
                new("code", code),
                new("client_id", _vkConfig.ClientId.ToString()),
                new("device_id", deviceId),
                new("state", Guid.NewGuid().ToString()[..12])
            };

        var authorizationResult = await _httpClient.PostAsync("/oauth2/auth", new FormUrlEncodedContent(formData));
        var authorizationData = await authorizationResult.GetData<AuthorizationResponse>(false, false);

        if (!authorizationData.IsError && authorizationData.Value is not null)
        {
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_ACCESSTOKEN_KEY, authorizationData.Value.AccessToken);
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_REFRESHTOKEN_KEY, authorizationData.Value.RefreshToken);
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_EXPIRETOKENDATE_KEY, DateTimeOffset.UtcNow.AddMinutes(40).ToUnixTimeSeconds().ToString());
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_DEVICEID_KEY, deviceId);

            return true;
        }

        return authorizationData.Errors;
    }

    public async Task<ErrorOr<string>> RefreshToken(string refreshToken, string deviceId)
    {
        var formData = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken),
                new("client_id", _vkConfig.ClientId.ToString()),
                new("device_id", deviceId),
                new("state", Guid.NewGuid().ToString()[..12])
            };

        var authorizationResult = await _httpClient.PostAsync("/oauth2/auth", new FormUrlEncodedContent(formData));
        var authorizationData = await authorizationResult.GetData<AuthorizationResponse>(false, false);
        
        if (!authorizationData.IsError && authorizationData.Value is not null)
        {
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_ACCESSTOKEN_KEY, authorizationData.Value.AccessToken);
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_REFRESHTOKEN_KEY, authorizationData.Value.RefreshToken);
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_EXPIRETOKENDATE_KEY, DateTimeOffset.UtcNow.AddMinutes(40).ToUnixTimeSeconds().ToString());
            await _localStorage.SetItemAsStringAsync(Constants.LOCALSTORAGE_VK_DEVICEID_KEY, deviceId);

            return authorizationData.Value.AccessToken;
        }

        return authorizationData.Errors;
    }

    public async Task<ErrorOr<bool>> Logout()
    {
        string? accessToken = await _localStorage.GetItemAsStringAsync(Constants.LOCALSTORAGE_VK_ACCESSTOKEN_KEY);
        if (string.IsNullOrWhiteSpace(accessToken)) { return false; }

        var formData = new List<KeyValuePair<string, string>>
            {
                new("client_id", _vkConfig.ClientId.ToString()),
                new("access_token", accessToken),

            };

        var authResult = await _httpClient.PostAsync("/oauth2/revoke", new FormUrlEncodedContent(formData));
        var data = await authResult.GetData<int>(true, false);

        if (!data.IsError && int.TryParse(data.Value.ToString(), out int responseInt) && responseInt == 1)
        {
            await _localStorage.RemoveItemsAsync(
            [
                Constants.LOCALSTORAGE_VK_ACCESSTOKEN_KEY,
                Constants.LOCALSTORAGE_VK_REFRESHTOKEN_KEY,
                Constants.LOCALSTORAGE_VK_EXPIRETOKENDATE_KEY,
                Constants.LOCALSTORAGE_VK_DEVICEID_KEY,
                Constants.LOCALSTORAGE_PKCE_VERIFIER
            ]);

            return true;
        }

        return data.FirstError;
    }

    public async Task<ErrorOr<string>> GetToken()
    {
        string? accessToken = await _localStorage.GetItemAsStringAsync(Constants.LOCALSTORAGE_VK_ACCESSTOKEN_KEY);
        string? refreshToken = await _localStorage.GetItemAsStringAsync(Constants.LOCALSTORAGE_VK_REFRESHTOKEN_KEY);
        string? deviceId = await _localStorage.GetItemAsStringAsync(Constants.LOCALSTORAGE_VK_DEVICEID_KEY);
        string? tokenUnixTime = await _localStorage.GetItemAsStringAsync(Constants.LOCALSTORAGE_VK_EXPIRETOKENDATE_KEY);

        if (string.IsNullOrWhiteSpace(accessToken) && string.IsNullOrWhiteSpace(refreshToken) && string.IsNullOrWhiteSpace(deviceId))
            return string.Empty;

        if (tokenUnixTime is not null && long.TryParse(tokenUnixTime, out long unixTime))
        {
            if (DateTime.UtcNow >= DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime)
            {
                var refreshTokenResult = await RefreshToken(refreshToken!, deviceId!);

                if (!refreshTokenResult.IsError && !string.IsNullOrWhiteSpace(refreshTokenResult.Value))
                    return refreshTokenResult.Value;

                await _localStorage.RemoveItemsAsync(
                [
                    Constants.LOCALSTORAGE_VK_ACCESSTOKEN_KEY,
                    Constants.LOCALSTORAGE_VK_REFRESHTOKEN_KEY,
                    Constants.LOCALSTORAGE_VK_EXPIRETOKENDATE_KEY,
                    Constants.LOCALSTORAGE_VK_DEVICEID_KEY
                ]);

                return refreshTokenResult.Errors;
            }
            else
            {
                return accessToken!;
            }
        }

        return string.Empty;
    }
}
