using WarringStates.Flow.Model;

namespace WarringStates.Flow;

public class SpanFlow : Flower
{
    public delegate void TickHandler(SpanFlowTickOnArgs args);

    public event TickHandler? Tick;

    DateStepper DateStepper { get; set; } = new();

    bool KeepFlow { get; set; } = false;

    long CurrentSpan { get; set; } = 0;

    Date CurrentDate => DateStepper.GetDate();

    public SpanFlow() : base(1000)
    {
        Timer.Elapsed += (_, _) => TickOn();
        Relocate(0);
    }

    public void Relocate(long startSpan)
    {
        //LocalEvents.Hub.ClearListener(LocalEvents.Flow.SwichFlowState);
        Stop();
        CurrentSpan = startSpan;
        DateStepper.SetStartSpan(CurrentSpan);
    }

    private void TickOn()
    {
        //LocalEvents.Hub.TryBroadcast(LocalEvents.Flow.SpanFlowTickOn, new SpanFlowTickOnArgs(CurrentSpan, CurrentDate));
        Timer.Stop();
        CurrentSpan++;
        DateStepper.StepOn();
        Tick?.Invoke(new(CurrentSpan, CurrentDate));
        Timer.Interval = GetInterval();
        if (KeepFlow)
            Timer.Start();
    }

    public void Start()
    {
        //LocalEvents.Hub.TryRemoveListener(LocalEvents.Flow.SwichFlowState, Start);
        //LocalEvents.Hub.TryAddListener(LocalEvents.Flow.SwichFlowState, Stop);
        KeepFlow = true;
        Timer.Start();
    }

    public void Stop()
    {
        //LocalEvents.Hub.TryRemoveListener(LocalEvents.Flow.SwichFlowState, Stop);
        //LocalEvents.Hub.TryAddListener(LocalEvents.Flow.SwichFlowState, Start);
        KeepFlow = false;
    }
}
