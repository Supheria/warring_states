using WarringStates.Map;

namespace WarringStates.User;

public class PlayerArchive()
{
    public string ArchiveId { get; set; } = "";

    public string WorldName { get; set; } = "";

    public Size WorldSize { get; set; } = new();

    public long CurrentSpan { get; set; } = 0;

    public VisibleLands VisibleLands { get; set; } = new();
}
