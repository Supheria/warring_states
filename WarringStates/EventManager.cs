namespace LocalUtilities.TypeToolKit.EventProcess;

public class EventManager
{
    public static EventManager Instance { get; } = new();

    EventHub EventHub { get; set; } = new();

    public void AddEvent(int eventId, IEventListener listener)
    {
        EventHub.AddListener(eventId, listener);
    }

    public void Dispatch(int eventId, IEventArgument argument)
    {
        EventHub.Dispatch(eventId, argument);
    }

    public void RemoveEvent(int eventId, IEventListener listener)
    {
        EventHub.RemoveListener(eventId, listener);
    }
}