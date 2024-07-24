using WarringStates.Events;

namespace WarringStates.Flow;

public class SpanFlowTickOnArgs(long currentSpan, Date currentDate) : ICallbackArgs
{
    public long CurrentSpan { get; } = currentSpan;

    public Date CurrentDate { get; } = currentDate;
}
