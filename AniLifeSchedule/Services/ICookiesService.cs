namespace AniLifeSchedule.Services
{
    public interface ICookiesService
    {
        /// <summary>
        /// Gets Access Token VK.
        /// Change this variable only in Razor Page (.cshtml), instead you gets exception.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
