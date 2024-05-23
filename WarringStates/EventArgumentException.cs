using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

public class EventArgumentException(string message) : ArgumentException(message)
{
    public static EventArgumentException ArgumentWrongType<TArgument, TListener>()
    {
        return new($"{typeof(TArgument).Name} is wrong argument type to {typeof(TListener).Name}");
    }
}
