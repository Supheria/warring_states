using WarringStates.Server.Map;

namespace WarringStates.Server.User;

internal class Archive(Size worldSize, RandomTable randomTable, List<LandPoint> landPoints)
{
    public long CurrentSpan { get; set; } = 0;

    public RandomTable RandomTable { get; set; } = randomTable;

    public Size WorldSize { get; set; } = worldSize;

    public List<LandPoint> LandPoints { get; set; } = landPoints;

    public Archive() : this(new(), new(), [])
    {

    }
}
