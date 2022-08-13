namespace AniLifeSchedule.Models.Configurations
{
    public class VKConfiguration
    {
        public string AuthorizeUrl { get; set; } = string.Empty;

       // public string AccessTokenUrl { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public string ClientSecret { get; set; } = string.Empty;

        public string RedirectUrl { get; set; } = string.Empty;

        public string GroupId { get; set; } = string.Empty;

        public string Display { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public string ResponseType { get; set; } = string.Empty;

        public string ApiVersion { get; set; } = string.Empty;
    }
}
