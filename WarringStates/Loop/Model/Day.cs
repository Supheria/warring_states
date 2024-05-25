namespace WarringStates.Loop.Model;

internal class Day(int max, int value)
{
    internal int Value { get; private set; } = value > 0 && value <= max ? value : throw LoopException.ValueOutRange<Day>(value);

    int Max { get; } = max;

    internal bool StepOn()
    {
        if (++Value > Max)
            return false;
        return true;
    }

    public override string ToString()
    {
        if (Value < 10)
            return $"0{Value}";
        return $"{Value}";
    }
}
