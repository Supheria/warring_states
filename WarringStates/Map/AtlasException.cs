using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Map;

public class AtlasException(string message) : Exception(message)
{
    public static AtlasException PointOutRange(Coordinate point)
    {
        return new($"{point} is out range of atlas map");
    }
}
