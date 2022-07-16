using AniLifeSchedule.Enums;
using AniLifeSchedule.Models.Shikimori;

namespace AniLifeSchedule.Services
{
    public interface IShikimoriService
    {
        /// <summary>
        /// Gets list of animes by id
        /// </summary>
        /// <param name="id">Id anime</param>
        /// <returns>List<Anime> object</returns>
        public Task<List<Anime>> GetAnime(int id);

        /// <summary>
        /// Gets list of animes by title
        /// </summary>
        /// <param name="title">Title</param>
        /// <returns>List<Anime> object</returns>
        public Task<List<Anime>> GetAnime(string title);

        /// <summary>
        /// Convert status string from Shikimori API to own Status Enum
        /// </summary>
        /// <param name="value">String status from Shikimori API</param>
        /// <returns>Status object with type Enum</returns>
        public Status GetStatusEnum(string value);

        /// <summary>
        /// Convert format string from Shikimori API to own Format Enum
        /// </summary>
        /// <param name="value">String format from Shikimori API</param>
        /// <returns>Format object with type Enum</returns>
        public Format GetFormatEnum(string value);
    }
}
