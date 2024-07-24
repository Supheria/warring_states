using WarringStates.Map.Terrain;

namespace WarringStates.Server.Map;

internal class SourceLands
{
    Dictionary<string, List<SourceLand>> SourceLandOwnerMap { get; } = [];

    public List<SourceLand> GetOwnership(string playerId)
    {
        if (SourceLandOwnerMap.TryGetValue(playerId, out var list))
            return list;

        return [];
    }
}
