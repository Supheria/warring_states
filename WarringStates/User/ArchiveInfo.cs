﻿using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class ArchiveInfo : IRosterItem<string>
{
    [TableField(IsPrimaryKey = true)]
    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public DateTime CreateTime { get; private set; }

    public DateTime LastSaveTime { get; private set; }

    public string Signature => Id;

    public ArchiveInfo(string worldName)
    {
        WorldName = worldName;
        LastSaveTime = CreateTime = DateTime.Now;
        Id = HashTool.ToMd5HashString(WorldName + CreateTime.ToBinary());
    }

    public ArchiveInfo()
    {

    }

    public void UpdateLastSaveTime()
    {
        LastSaveTime = DateTime.Now;
    }
}
