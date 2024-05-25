namespace WarringStates.Loop.Model;

internal class Calendar
{
    Dictionary<int, Date> SpanMap { get; } = [];

    internal static int SpanMax => 3650000;

    internal Date this[int span]
    {
        get
        {
            if (SpanMap.TryGetValue(span, out var date))
                return date;
            throw LoopException.ValueOutRange<Calendar>(span);
        }
    }

    internal Calendar()
    {
        var stepper = new DateStepper();
        var dateType = DateType.Monday;
        for (var i = 1; i <= SpanMax; i++)
        {
            var date = stepper.Get(dateType);
            SpanMap[i] = date;
            stepper.StepOn();
            if (dateType is DateType.Sunday)
                dateType = DateType.Monday;
            else
                dateType++;
        }
    }
}
