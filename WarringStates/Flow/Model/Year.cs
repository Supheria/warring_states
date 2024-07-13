namespace WarringStates.Flow.Model;

internal class Year
{
    internal int Value { get; private set; }

    internal bool IsLeap { get; private set; }

    internal Year(int value)
    {
        Value = value < 0 ? throw FlowException.ValueOutRange<Year>(value) : value;
        SetIsLeap();
    }

    private void SetIsLeap()
    {
        if (Value % 100 == 0)
        {
            if (Value / 100 % 4 == 0)
                IsLeap = true;
            else
                IsLeap = false;
        }
        else
        {
            if (Value % 4 == 0)
                IsLeap = true;
            else
                IsLeap = false;
        }
    }

    public static Year operator ++(Year year)
    {
        year.Value++;
        year.SetIsLeap();
        return year;
    }
}
