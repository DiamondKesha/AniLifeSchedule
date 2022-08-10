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

        public bool Unsubscribe<T>(T type);

        public void Notify<T>(T type);

        public Task NotifyAsync<T>(T type);
    }
}
