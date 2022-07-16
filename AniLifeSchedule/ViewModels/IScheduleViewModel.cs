using AniLifeSchedule.Models;
using System.Collections.ObjectModel;

namespace AniLifeSchedule.ViewModels
{
    public interface IScheduleViewModel
    {
        /// <summary>
        /// Storing Date from DateInput
        /// </summary>
        public DateTime DateInput { get; set; }

        /// <summary>
        /// Storing input data like a id or title anime for method "Add"
        /// Not used if the this field is null or empty
        /// </summary>
        public string ShikimoriInput { get; set; }

        /// <summary>
        /// Storing data from Model
        /// </summary>
        public ObservableCollection<ScheduleModel> Models { get; set; }

        /// <summary>
        /// Storing Base64 data after generating image
        /// </summary>
        public string OutputImageData { get; set; }

        /// <summary>
        /// Gets schedule from API Anilist and then adds items in Model
        /// </summary>
        public Task GetSchedule();

        /// <summary>
        /// Adds new item in Model
        /// </summary>
        public Task AddItem();

        /// <summary>
        /// Gets Russian title from shikimori API
        /// </summary>
        public Task GetRussianTitle(int id);

        /// <summary>
        /// Generates the image
        /// </summary>
        public Task Generate();

        /// <summary>
        /// Sorts Model by date and time
        /// </summary>
        public void Sort();
    }
}
