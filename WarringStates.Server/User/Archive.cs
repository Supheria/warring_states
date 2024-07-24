using AltitudeMapGenerator;
using WarringStates.Map;
using WarringStates.Server.Map;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class Archive
{
    public AltitudeMap AltitudeMap { get; set; }

    public RandomTable RandomTable { get; set; }

    public SourceLands SourceLands { get; set; } = new();

    public Players Players { get; set; } = [];

    public int CurrentSpan { get; set; } = 0;

    public Archive(/*ArchiveInfo info, */AltitudeMap altitudeMap, RandomTable randomTable)
    {
        //Info = info;
        AltitudeMap = altitudeMap;
        RandomTable = randomTable;
    }

    public Archive() : this(/*new(), */new(), new())
    {

    }
}
