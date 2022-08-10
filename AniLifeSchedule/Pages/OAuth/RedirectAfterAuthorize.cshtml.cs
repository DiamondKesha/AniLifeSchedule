using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AniLifeSchedule.Pages.OAuth
{
    public class GetAccessTokenModel : PageModel
    {
        public ContentResult OnGet()
        {
            return Content("<html><script>window.location.href=window.location.href.replace('#', '?').replace('RedirectAfterAuthorize', 'HandlingAccessToken')</script></html>", "text/html");
        }
    }
}
