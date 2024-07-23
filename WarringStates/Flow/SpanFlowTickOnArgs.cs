using WarringStates.Events;
using WarringStates.Flow;

namespace WarringStates.Flow;

public class SpanFlowTickOnArgs(int currentSpan, Date currentDate) : ICallbackArgs
{
    public int CurrentSpan { get; } = currentSpan;

    public Date CurrentDate { get; } = currentDate;
}
