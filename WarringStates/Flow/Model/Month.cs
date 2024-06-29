namespace WarringStates.Flow.Model;

internal class Month
{
    internal int Value { get; private set; }

    internal int DayMax { get; private set; }

    int FebruaryDayMax { get; }

    internal Month(bool isLeap, int value)
    {
        Value = value < 1 || value > 12 ? throw FlowException.ValueOutRange<Month>(value) : value;
        FebruaryDayMax = isLeap ? 29 : 28;
        SetDayMax();
    }

    internal void SetDayMax()
    {
        if (Value is 1 || Value is 3 || Value is 5 || Value is 7 || Value is 8 || Value is 10 || Value is 12)
            DayMax = 31;
        else if (Value == 2)
            DayMax = FebruaryDayMax;
        else
            DayMax = 30;
    }

    internal bool StepOn()
    {
        if (++Value > 12)
            return false;
        SetDayMax();
        return true;
    }
}
