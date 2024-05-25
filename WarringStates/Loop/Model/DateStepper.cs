namespace WarringStates.Loop.Model;

internal class DateStepper
{
    Year Year { get; set; }

    Month Month { get; set; }

    Day Day { get; set; }

    internal DateStepper()
    {
        Year = new(1);
        Month = new(Year.IsLeap, 1);
        Day = new(Month.DayMax, 1);
    }

    internal void StepOn()
    {
        if (!Day.StepOn())
        {
            if (!Month.StepOn())
            {
                Year++;
                Month = new(Year.IsLeap, 1);
            }
            Day = new(Month.DayMax, 1);
        }
    }

    internal Date Get(DateType dateType)
    {
        return new(Year.Value, Month.Value, Day.Value, dateType);
    }
}
