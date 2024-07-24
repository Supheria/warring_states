using WarringStates.Map.Terrain;

namespace WarringStates.User;

public class PlayerArchive()
{
    public string ArchiveId { get; set; } = "";

    public string WorldName { get; set; } = "";

    public Size WorldSize { get; set; } = new();

    public long CurrentSpan { get; set; } = 0;

    public int PlayerCount { get; set; } = 0;

    public List<SourceLand> OwnerShip { get; set; } = [];
}
