using WarringStates.Events;
using WarringStates.UI;
using Timer = System.Windows.Forms.Timer;

namespace WarringStates.Loop.Model;

internal class Tick
{
    DateStepper DateStepper { get; set; } = new();

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
        DateStepper.StepOn();
        LocalEvents.Global.Broadcast(LocalEventTypes.Global.TimeTickOn, CurrentSpan);

        var date = DateStepper.GetDate();
        var info = new TestForm.TestInfo("time", date.ToString());
        LocalEvents.Test.Broadcast(LocalEventTypes.Test.AddInfo, info);
    }

    internal Date GetDate()
    {
        return DateStepper.GetDate();
    }

    private void Start()
    {
        LocalEvents.Loop.TryRemoveListener(LocalEventTypes.Loop.StartSpanFlow, Start);
        LocalEvents.Loop.AddListener(LocalEventTypes.Loop.StopSpanFlow, Stop);
        Timer.Start();
    }

    private void Stop()
    {
        LocalEvents.Loop.TryRemoveListener(LocalEventTypes.Loop.StopSpanFlow, Stop);
        LocalEvents.Loop.AddListener(LocalEventTypes.Loop.StartSpanFlow, Start);
        Timer.Stop();
    }

    internal void Relocate(int startSpan)
    {
        LocalEvents.Loop.ClearListener(LocalEventTypes.Loop.StartSpanFlow);
        LocalEvents.Loop.ClearListener(LocalEventTypes.Loop.StopSpanFlow);
        Stop();
        CurrentSpan = startSpan;
        DateStepper.SetStartDate(CurrentSpan);
    }
}
