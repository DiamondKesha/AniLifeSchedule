using AniLifeSchedule.Models.Configurations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AniLifeSchedule.Pages.OAuth
{
    public class HandlingAccessTokenModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly VKConfiguration _vkAuth;

        public string Message { get; set; }
        public bool IsAuthorized { get; set; }

        public HandlingAccessTokenModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IOptions<VKConfiguration> vkAuth)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _vkAuth = vkAuth.Value;
        }

        public void OnGet()
        {
            string token = _httpContextAccessor.HttpContext.Request.Query[$"access_token"];
            string expiresIn = _httpContextAccessor.HttpContext.Request.Query[$"expires_in"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(expiresIn))
            {
                Message = $"Error: {_httpContextAccessor.HttpContext.Request.Query[$"error"]}"
                        + $"Description: {_httpContextAccessor.HttpContext.Request.Query[$"error_description"]}";
                IsAuthorized = false;

                return;
            }

            Message = $"Token: {token}"
                    + $"Tiken is expires: {expiresIn}"
                    + "All data is saved in cookies with name VKToken, VKTokenExpires";
            IsAuthorized = true;

            Response.Cookies.Append("VKToken", token);
            Response.Cookies.Append("VKTokenExpires", expiresIn);
        }
    }
}
