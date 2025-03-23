namespace AniLifeSchedule.Contracts.Services;

public interface IVKAuthService
{
    public Task<ErrorOr<bool>> Authorize(string code, string deviceId);

    public Task<ErrorOr<string>> RefreshToken(string refreshToken, string deviceId);

    public Task<ErrorOr<string>> GetToken();

    public Task<ErrorOr<bool>> Logout();
}
