using LocalUtilities.General;

namespace WarringStates.Map;

public class AtlasData
{
    public Size WorldSize { get; set; } = new();

    public VisibleLands VisibleLands { get; set; } = new();
}
