namespace WarringStates.Loop.Model;

internal class Year
{
    internal int Value { get; private set; }

    internal bool IsLeap { get; private set; }

    internal Year(int value)
    {
        Value = value > 0 ? value : throw LoopException.ValueOutRange<Year>(value);
        SetIsLeap();
    }

    private void SetIsLeap()
    {
        if (Value % 100 is 0 && Value / 100 % 4 is 0)
            IsLeap = true;
        else if (Value % 4 == 0)
            IsLeap = true;
        IsLeap = false;
    }

    public static Year operator ++(Year year)
    {
        year.Value++;
        year.SetIsLeap();
        return year;
    }
}
