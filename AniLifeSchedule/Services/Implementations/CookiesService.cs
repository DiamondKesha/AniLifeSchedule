namespace AniLifeSchedule.Services.Implementations
{
    public class CookiesService : ICookiesService
    {
        private IHttpContextAccessor _httpContext;

        public CookiesService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string AccessToken
        {
            get => _httpContext.HttpContext.Request.Cookies["VKToken"];
            set => _httpContext.HttpContext.Response.Cookies.Append("VKToken", value);
        }
    }
}
