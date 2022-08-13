namespace AniLifeSchedule.Services
{
    public interface INotifierService
    {
        /// <summary>
        /// Subcribe page or component to Notify Service
        /// </summary>
        /// <typeparam name="T">Service or Type of component for identification notify</typeparam>
        /// <param name="type">Identification type</param>
        /// <param name="notifyEvent">Event which will be invoke</param>
        /// <returns>True is subscribe is successfully; False is failed</returns>
        public bool Subscribe<T>(T type, Func<Task> notifyEvent);

        /// <summary>
        /// Unscribe page or component from Notify Service
        /// </summary>
        /// <typeparam name="T">Service or Type of component for identification notify</typeparam>
        /// <param name="type">Identification type</param>
        /// <returns>True or False</returns>
        public bool Unsubscribe<T>(T type);

        /// <summary>
        /// Notify if objects in page or component has changed.
        /// Method which that was specified in Subrscribe method will be execute.
        /// </summary>
        /// <typeparam name="T">Identification type</typeparam>
        public void Notify<T>(T type);

        /// <summary>
        /// Notify if objects in page or component has changed.
        /// Method which that was specified in Subrscribe method will be execute.
        /// </summary>
        /// <typeparam name="T">Identification type</typeparam>
        public Task NotifyAsync<T>(T type);
    }
}
