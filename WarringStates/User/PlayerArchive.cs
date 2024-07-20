using WarringStates.Map.Terrain;

namespace WarringStates.User;

public class PlayerArchive(string id, string worldName, Size worldSize, int playerCount, List<SourceLand> ownSourceLands)
{
    public string Id { get; private set; } = id;

    public string WorldName { get; private set; } = worldName;

    public Size WorldSize { get; private set; } = worldSize;

    public int PlayerCount { get; private set; } = playerCount;

    public List<SourceLand> OwnLands { get; private set; } = ownSourceLands;

    public PlayerArchive() : this("", "", new(), 0, [])
    {

    }
}
