namespace WarringStates.Flow;

public class AnimateFlow : Flower
{
    public delegate void TickHandler();

    public event TickHandler? Tick;

    public AnimateFlow() : base(20)
    {
        Timer.Elapsed += (_, _) => TickOn();
        Timer.Start();
    }

    private void TickOn()
    {
        Tick?.Invoke();
        //LocalEvents.Hub.TryBroadcast(LocalEvents.Flow.AnimateFlowTickOn);
        Timer.Stop();
        Timer.Interval = GetInterval();
        Timer.Start();
    }
}
