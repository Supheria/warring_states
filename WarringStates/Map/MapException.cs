using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Map;

public class MapException : Exception
{
    public MapException(string message) : base(message)
    {

    }
}
