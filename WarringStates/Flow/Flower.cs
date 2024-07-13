using Timer = System.Timers.Timer;

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

    double IntervalNormal { get; }

    double IntervalHalf { get; }

    double Interval2x { get; }

    double Interval3x { get; }

    protected Flower(double normalInterval)
    {
        IntervalNormal = normalInterval;
        IntervalHalf = normalInterval * 2d;
        Interval2x = normalInterval / 2d;
        Interval3x = normalInterval / 3d;
    }

    protected double GetInterval()
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
