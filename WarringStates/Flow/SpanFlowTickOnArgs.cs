namespace WarringStates.Flow;

public class SpanFlowTickOnArgs(long currentSpan, Date currentDate) : EventArgs
{
    public long CurrentSpan { get; private set; } = currentSpan;

    public Date CurrentDate { get; private set; } = currentDate;

    public SpanFlowTickOnArgs() : this(0, new())
    {

    }
}
