using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeToolKit;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Map.Terrain;
using WarringStates.Map;
using WarringStates.User;
using LocalUtilities.TypeGeneral;

namespace WarringStates.User;

public class ArchiveInfo : RosterItem<string>
{
    [TableField(IsPrimaryKey = true)]
    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public Size WorldSize { get; private set; } = new();

    public DateTime CreateTime { get; private set; }

    public DateTime LastSaveTime { get; private set; }

    public override string Signature => Id;

    public ArchiveInfo(string worldName, Size worldSize)
    {
        WorldName = worldName;
        WorldSize = worldSize;
        LastSaveTime = CreateTime = DateTime.Now;
        Id = (WorldName + CreateTime).ToMd5HashString();
    }

    public ArchiveInfo()
    {

    }

    public void UpdateLastSaveTime()
    {
        LastSaveTime = DateTime.Now;
    }
}
