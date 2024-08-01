using LocalUtilities.TypeToolKit.Convert;
using System.Text;

namespace WarringStates.Flow;

public struct Date(int year, int month, int day, DateType type)
{
    public int Year { get; private set; } = year;

    public int Month { get; private set; } = month;

    public int Day { get; private set; } = day;

    public DateType Type { get; private set; } = type;

    public Date() : this(0, 0, 0, DateType.Monday)
    {

    }

    public override string ToString()
    {
        return new StringBuilder()
            .Append(ToString(Year))
            .Append('.')
            .Append(ToString(Month))
            .Append('.')
            .Append(ToString(Day))
            .Append(':')
            .Append(' ')
            .Append(Type.GetDescription())
            .ToString();
    }

    private static string ToString(int value)
    {
        if (value < 10)
            return $"0{value}";
        if (value < 100)
            return $"{value}";
        if (value < 1000)
            return $"{value}";
        return $"{value}";
    }
}
