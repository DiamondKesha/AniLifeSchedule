namespace AniLifeSchedule.Services.Implementations;

public class CookiesService : ICookiesService
{
    private readonly IHttpContextAccessor _httpContext = default!;

    public CookiesService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public string AccessToken
    {
        get => _httpContext.HttpContext?.Request.Cookies["VKToken"] ?? string.Empty;
        set => _httpContext.HttpContext?.Response.Cookies.Append("VKToken", value);
    }
}
