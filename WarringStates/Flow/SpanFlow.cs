using WarringStates.Events;
using WarringStates.Flow.Model;
using WarringStates.UI;
using Timer = System.Windows.Forms.Timer;

namespace WarringStates.Flow;

public enum Speed
{
    Normal,
    Half,
    X2,
    X3,
    ForTest
}

public class SpanFlow
{
    DateStepper DateStepper { get; set; } = new();

    Timer Timer { get; } = new();

    bool KeepFlow { get; set; } = false;

    public int CurrentSpan { get; private set; } = 0;

    public Date CurrentDate => DateStepper.GetDate();

    public Speed Speed { get; set; } = Speed.Normal;

    public SpanFlow()
    {
        Timer.Tick += (sender, e) => TickOn();
    }

    public void Relocate(int startSpan)
    {
        LocalEvents.Hub.ClearListener(LocalEvents.Flow.SwichFlowState);
        Stop();
        CurrentSpan = startSpan;
        DateStepper.SetStartSpan(CurrentSpan);
    }

    private void TickOn()
    {
        CurrentSpan++;
        DateStepper.StepOn();
        LocalEvents.Hub.TryBroadcast(LocalEvents.Flow.SpanFlowTickOn, new SpanFlowTickOnArgs(CurrentSpan, CurrentDate));
        Timer.Stop();
        Timer.Interval = Speed switch
        {
            Speed.Half => 2000,
            Speed.X2 => 500,
            Speed.X3 => 333,
            Speed.ForTest => 1,
            _ => 1000,
        };
        if (KeepFlow)
            Timer.Start();
    }

    private void Start()
    {
        LocalEvents.Hub.TryRemoveListener(LocalEvents.Flow.SwichFlowState, Start);
        LocalEvents.Hub.AddListener(LocalEvents.Flow.SwichFlowState, Stop);
        KeepFlow = true;
        Timer.Start();
    }

    private void Stop()
    {
        LocalEvents.Hub.TryRemoveListener(LocalEvents.Flow.SwichFlowState, Stop);
        LocalEvents.Hub.AddListener(LocalEvents.Flow.SwichFlowState, Start);
        KeepFlow = false;
    }
}
