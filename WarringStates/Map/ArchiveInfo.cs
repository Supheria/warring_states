namespace WarringStates.Map;
using LocalUtilities.General;

public class ArchiveInfo
{
    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public DateTime CreateTime { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public long CurrentSpan { get; set; }

    public ArchiveInfo(string worldName, int width, int height)
    {
        WorldName = worldName;
        CreateTime = DateTime.Now;
        Id = HashTool.ToMd5HashString(WorldName + CreateTime.ToBinary());
        Width = width;
        Height = height;
        CurrentSpan = 0;
    }

    public ArchiveInfo()
    {

    }

    public override string ToString()
    {
        return $"{WorldName}";
    }
}
