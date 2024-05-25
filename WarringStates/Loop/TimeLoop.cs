using LocalUtilities.SimpleScript.Serialization;
using WarringStates.Events;
using WarringStates.Loop.Model;

namespace WarringStates.Loop;

public static class TimeLoop
{
    static Tick Tick { get; set; } = new Tick();

    public static void Start()
    {
        LocalEvents.Loop.Broadcast(LocalEvents.Types.Loop.StartSpanFlow);
    }

    public static void Stop()
    {
        LocalEvents.Loop.Broadcast(LocalEvents.Types.Loop.StopSpanFlow);
    }

    public static void Relocate(int startSpan)
    {
        Tick.Relocate(startSpan);
    }

    public static int GetCurrentSpan()
    {
        return Tick.CurrentSpan;
    }

    public static Date GetDate(int span)
    {
        return Tick.GetDate(span);
    }
}
