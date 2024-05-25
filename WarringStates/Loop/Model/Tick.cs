using WarringStates.Events;
using WarringStates.UI;
using Timer = System.Windows.Forms.Timer;

namespace WarringStates.Loop.Model;

internal class Tick
{
    Calendar Calendar { get; set; } = new();

    Timer Timer { get; } = new();

    internal int CurrentSpan { get; private set; } = 0;

    internal int Interval
    {
        get => _interval;
        set
        {
            _interval = value;
            Timer.Interval = value;
        }
    }
    int _interval = 1000;

    public Tick()
    {
        Timer.Interval = Interval;
        Timer.Tick += TickOn;
    }

    private void TickOn(object? sender, EventArgs e)
    {
        CurrentSpan++;
        LocalEvents.Global.Broadcast(LocalEvents.Types.Global.TimeTickOn, CurrentSpan);

        var date = Calendar[CurrentSpan];
        var info = new TestForm.TestInfo("time", date.ToString());
        LocalEvents.Test.Broadcast(LocalEvents.Types.Test.AddInfo, info);
    }

    internal Date GetDate(int span)
    {
        return Calendar[span];
    }

    private void Start()
    {
        LocalEvents.Loop.TryRemoveListener(LocalEvents.Types.Loop.StartSpanFlow, Start);
        LocalEvents.Loop.AddListener(LocalEvents.Types.Loop.StopSpanFlow, Stop);
        Timer.Start();
    }

    private void Stop()
    {
        LocalEvents.Loop.TryRemoveListener(LocalEvents.Types.Loop.StopSpanFlow, Stop);
        LocalEvents.Loop.AddListener(LocalEvents.Types.Loop.StartSpanFlow, Start);
        Timer.Stop();
    }

    internal void Relocate(int startSpan)
    {
        LocalEvents.Loop.ClearListener(LocalEvents.Types.Loop.StartSpanFlow);
        LocalEvents.Loop.ClearListener(LocalEvents.Types.Loop.StopSpanFlow);
        Stop();
        CurrentSpan = startSpan;
    }
}
