namespace AniLifeSchedule.Models
{
    public class AuthorizeModel
    {
        public bool IsAuthorized { get; set; }

        public string NameGroup { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int MembersCount { get; set; }
    }
}
