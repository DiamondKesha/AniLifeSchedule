using AniLifeSchedule.Models.Configurations;
using AniLifeSchedule.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AniLifeSchedule.Pages.OAuth
{
    public class AuthorizeModel : PageModel
    {
        private readonly ICookiesService _cookieService;
        private readonly VKConfiguration _vkAuth;

        public string UrlAuthorization { get; set; } = string.Empty;
        public string AccessTokenValue { get; set; } = string.Empty;

        public AuthorizeModel(IOptions<VKConfiguration> vkAuth, ICookiesService cookieService)
        {
            _cookieService = cookieService;
            _vkAuth = vkAuth.Value;

            UrlAuthorization = _vkAuth.AuthorizeUrl
                + $"client_id={_vkAuth.ClientId}"
                + $"&display={_vkAuth.Display}"
                + $"&redirect_uri={_vkAuth.RedirectUrl}"
                //+ $"&group_ids={_vkAuth.GroupId}"
                + $"&scope={_vkAuth.Scope}"
                + $"&response_type={_vkAuth.ResponseType}"
                + $"&v={_vkAuth.ApiVersion}";
        }

        public async Task OnPostAsync(string AccessTokenValue)
        {
            if (!string.IsNullOrEmpty(AccessTokenValue)) _cookieService.AccessToken = AccessTokenValue;
        }

        //public IActionResult OnGet()
        //{
        /*return Redirect(_vkAuth.AuthorizeUrl
            + $"client_id={_vkAuth.ClientId}"
            + $"&display={_vkAuth.Display}"
            + $"&redirect_uri={_vkAuth.RedirectUrl}"
            //+ $"&group_ids={_vkAuth.GroupId}"
            + $"&scope={_vkAuth.Scope}"
            + $"&response_type={_vkAuth.ResponseType}"
            + $"&v={_vkAuth.ApiVersion}");*/


        //}
    }
}
