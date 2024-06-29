using WarringStates.Events;
using Timer = System.Windows.Forms.Timer;

namespace WarringStates.Flow;

public class AnimateFlow : Flower
{
    public AnimateFlow() : base(20)
    {
        Timer.Elapsed += (_, _) => TickOn();
        Timer.Start();
    }

    private void TickOn()
    {
        LocalEvents.Hub.TryBroadcast(LocalEvents.Flow.AnimateFlowTickOn);
        Timer.Stop();
        Timer.Interval = GetInterval();
        Timer.Start();
    }
}
