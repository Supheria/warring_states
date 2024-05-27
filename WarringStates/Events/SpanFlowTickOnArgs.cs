using WarringStates.Flow;

namespace WarringStates.Events;

public class SpanFlowTickOnArgs(int currentSpan, Date currentDate)
{
    public int CurrentSpan { get; } = currentSpan;

    public Date CurrentDate { get; } = currentDate;
}
