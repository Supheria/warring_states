﻿namespace WarringStates.Map;
using LocalUtilities.General;

public class ArchiveInfo
{
    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public DateTime CreateTime { get; private set; }

    public Size WorldSize { get; private set; } = new();

    public long CurrentSpan { get; set; }

    public ArchiveInfo(string worldName, Size worldSize)
    {
        WorldName = worldName;
        CreateTime = DateTime.Now;
        Id = HashTool.ToMd5HashString(WorldName + CreateTime.ToBinary());
        WorldSize = worldSize;
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
