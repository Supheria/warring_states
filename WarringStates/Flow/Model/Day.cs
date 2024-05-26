namespace WarringStates.Flow.Model;

internal class Day(int max, int value)
{
    internal int Value { get; private set; } = value < 1 || value > max ? throw LoopException.ValueOutRange<Day>(value) : value;

    int Max { get; } = max;

    internal bool StepOn()
    {
        if (++Value > Max)
            return false;
        return true;
    }
}
