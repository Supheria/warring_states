using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace WarringStates.Flow;

public class Flower
{
    public enum Type
    {
        Normal,
        Half,
        X2,
        X3,
        ForTest
    }

    public static Type Speed { get; set; } = Type.Normal;

    protected Timer Timer { get; } = new();

    int IntervalNormal { get; }

    int IntervalHalf { get; }

    int Interval2x { get; }

    int Interval3x { get; }

    protected Flower(int normalInterval)
    {
        IntervalNormal = normalInterval;
        IntervalHalf = normalInterval * 2;
        Interval2x = (normalInterval * 0.5).ToRoundInt();
        Interval3x = (normalInterval * 0.333).ToRoundInt();
    }

    protected int GetInterval()
    {
        return Speed switch
        {
            Type.Half => IntervalHalf,
            Type.X2 => Interval2x,
            Type.X3 => Interval3x,
            Type.ForTest => 1,
            _ => IntervalNormal
        };
    }
}
