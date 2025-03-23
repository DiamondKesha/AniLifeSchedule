using System.Text.Json.Serialization;

namespace AniLifeSchedule.Models.VK.OAuth;

public class AuthorizationRequest
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; } = "authorization_code";

    [JsonPropertyName("code")]
    public string Code { get; } = string.Empty;

    [JsonPropertyName("code_verifier")]
    public string CodeVerifier { get; } = string.Empty;

    [JsonPropertyName("client_id")]
    public long ClientId { get; }

    [JsonPropertyName("device_id")]
    public string DeviceId { get; } = string.Empty;

    [JsonPropertyName("redirect_uri")]
    public string RedirectUrl { get; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; } = string.Empty;

    public AuthorizationRequest(string code, string codeVerifier, long clientId, string deviceId, string redirectUrl, string state)
    {
        Code = code;
        CodeVerifier = codeVerifier;
        ClientId = clientId;
        DeviceId = DeviceId;
        RedirectUrl = redirectUrl;
        State = state;
    }

    public static AuthorizationRequest Create(string code, string codeVerifier, long clientId, string deviceId, string redirectUrl, string state)
    {
        return new AuthorizationRequest(code, codeVerifier, clientId, deviceId, redirectUrl, state);
    }
}
