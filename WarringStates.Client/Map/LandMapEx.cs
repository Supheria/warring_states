using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Client.Map;

internal class LandMapEx : LandMap
{
    public override Land this[Coordinate point]
    {
        get
        {
            if (SourceLands.TryGetValue(point, out var sourceLand))
                return sourceLand;
            if (SingleLands.TryGetValue(point, out var singleLand))
                return singleLand;
            return new SingleLand(point, LandTypes.None);
        }
    }

    public void Relocate(VisibleLands visibleLands, Size worldSize)
    {
        SingleLands.Clear();
        SingleLands.AddArange(visibleLands.SingleLands);
        SourceLands.Clear();
        SourceLands.AddArange(visibleLands.SourceLands);
        WorldSize = worldSize;
    }
}
