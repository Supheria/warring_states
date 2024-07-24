using AltitudeMapGenerator;
using WarringStates.Map;
using WarringStates.Server.Map;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class Archive(AltitudeMap altitudeMap, RandomTable randomTable)
{
    public AltitudeMap AltitudeMap { get; set; } = altitudeMap;

    public RandomTable RandomTable { get; set; } = randomTable;

    public SourceLands SourceLands { get; set; } = new();

    public Players Players { get; set; } = [];

    public long CurrentSpan { get; set; } = 0;

    public Archive() : this(new(), new())
    {

    }
}
