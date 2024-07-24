namespace WarringStates.Flow.Model;

public class DateStepper
{
    Year Year { get; set; }

    Month Month { get; set; }

    Day Day { get; set; }

    DateType DateType { get; set; }

    public DateStepper()
    {
        Year = new(1);
        Month = new(Year.IsLeap, 1);
        Day = new(Month.DayMax, 1);
        DateType = DateType.Monday;
    }

    public void SetStartSpan(long spanTo01_01_01)
    {
        Year = new(1);
        Month = new(Year.IsLeap, 1);
        Day = new(Month.DayMax, 1);
        DateType = DateType.Monday;
        for (var i = 1; i < spanTo01_01_01; i++)
            StepOn();
    }

    public void StepOn()
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
        if (DateType is DateType.Sunday)
            DateType = DateType.Monday;
        else
            DateType++;
    }

    public Date GetDate()
    {
        return new(Year.Value, Month.Value, Day.Value, DateType);
    }
}
