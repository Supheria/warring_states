using LocalUtilities.General;

namespace WarringStates.Map;

public class SourceLandCanBuildArgs(Coordinate site, SourceLandTypes[] canBuildTypes) : EventArgs
{
    public Coordinate Site { get; private set; } = site;

    public SourceLandTypes[] CanbuildTypes { get; private set; } = canBuildTypes;

    public SourceLandCanBuildArgs() : this(new(), [])
    {

    }
}
