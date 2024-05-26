using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Flow;

namespace WarringStates.Events;

public class SpanFlowTickOnArgs(int currentSpan, Date currentDate)
{
    public int CurrentSpan { get; } = currentSpan;

    public Date CurrentDate { get; } = currentDate;
}
