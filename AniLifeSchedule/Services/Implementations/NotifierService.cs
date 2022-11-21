namespace AniLifeSchedule.Services.Implementations;

public class NotifierService : INotifierService
{
    private IDictionary<Type, Func<Task>> subscribers;

    public NotifierService()
    {
        subscribers = new Dictionary<Type, Func<Task>>();
    }

    public void Notify<T>(T type)
    {
        foreach (var item in subscribers)
        {
            if (item.Key == type?.GetType()) item.Value?.Invoke();
        }
    }

    public async Task NotifyAsync<T>(T type)
    {
        foreach (var item in subscribers)
        {
            if (item.Key == type?.GetType()) await item.Value?.Invoke();
        }
    }

    public bool Subscribe<T>(T type, Func<Task> notifyEvent)
    {
        if (subscribers.ContainsKey(type.GetType())) return false;

        subscribers.Add(type.GetType(), notifyEvent);
        return true;
    }

    public bool Unsubscribe<T>(T type)
    {
        var subscriber = subscribers.FirstOrDefault(x => x.Key == type?.GetType());

        if (!subscriber.Equals(default)) return false;

        subscribers.Remove(type?.GetType());
        return true;
    }
}