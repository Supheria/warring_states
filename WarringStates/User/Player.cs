using LocalUtilities.TypeGeneral;

namespace WarringStates.User;

public class Player
{
    public string ArchiveId { get; }

    public string Name { get; }

    public HashSet<Coordinate> SouceLandPoints { get; } = [];

    public Coordinate GridOrigin { get; private set; } = new();
}
