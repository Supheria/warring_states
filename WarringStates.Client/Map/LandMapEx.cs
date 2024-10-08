﻿using LocalUtilities.TypeGeneral;
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
            return new SingleLand(point, SingleLandTypes.None);
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

    public void AddVision(VisibleLands visibleLands)
    {
        SingleLands.AddArange(visibleLands.SingleLands);
        SourceLands.AddArange(visibleLands.SourceLands);
    }
}
