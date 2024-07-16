using WarringStates.Map.Terrain;

namespace WarringStates.User;

public class PlayerArchiveInfo(string id, string worldName, Size worldSize, int playerCount, List<SourceLand> ownSourceLands)
{
    public string Id { get; private set; } = id;

    public string WorldName { get; private set; } = worldName;

    public Size WorldSize { get; private set; } = worldSize;

    public int PlayerCount { get; private set; } = playerCount;

    public List<SourceLand> OwnSourceLands { get; private set; } = ownSourceLands;

    public PlayerArchiveInfo() : this("", "", new(), 0, [])
    {

    }
}
