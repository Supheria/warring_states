using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map.Terrain;

namespace WarringStates.Server.Map;

internal class SourceLands
{
    Dictionary<string, List<SourceLand>> SourceLandOwnerMap { get; } = [];

    public List<SourceLand> GetOwnership(string userId)
    {
        if (SourceLandOwnerMap.TryGetValue(userId, out var list))
            return list;

        return [];
    }
}
