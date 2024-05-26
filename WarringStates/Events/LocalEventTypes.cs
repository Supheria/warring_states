using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Events;

internal class LocalEventTypes
{
    public enum Test
    {
        AddInfo,
        AddInfoList,
    }

    public enum Global
    {
        TimeTickOn,
        GameFormUpdate,
        ImageUpdate,
        GridUpdate
    }

    public enum Loop
    {
        StopSpanFlow,
        StartSpanFlow,
    }
}
