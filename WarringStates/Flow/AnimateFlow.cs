using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;
using WarringStates.Flow.Model;
using Timer = System.Windows.Forms.Timer;

namespace WarringStates.Flow;

public class AnimateFlow : Flower
{
    public AnimateFlow() : base(20)
    {
        Timer.Tick += (sender, e) => TickOn();
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
