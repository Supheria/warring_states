using WarringStates.Events;
using WarringStates.Flow.Model;

namespace WarringStates.Flow;

public class SpanFlow : Flower
{
    DateStepper DateStepper { get; set; } = new();

    bool KeepFlow { get; set; } = false;

    public int CurrentSpan { get; private set; } = 0;

    public Date CurrentDate => DateStepper.GetDate();

    public SpanFlow() : base(1000)
    {
        Timer.Tick += (sender, e) => TickOn();
        Relocate(0);
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
        LocalEvents.Hub.TryBroadcast(LocalEvents.Flow.SpanFlowTickOn, new SpanFlowTickOnArgs(CurrentSpan, CurrentDate));
        CurrentSpan++;
        DateStepper.StepOn();
        Timer.Stop();
        Timer.Interval = GetInterval();
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
