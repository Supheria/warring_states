using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.Map;

public class ArchiveInfo
{
    public const string ARCHIVE_INFO = "archive info";

    public static string RootPath { get; } = Directory.CreateDirectory("saves").FullName;

    [TableField(IsPrimaryKey = true)]
    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public DateTime CreateTime { get; private set; }

    public DateTime LastSaveTime { get; private set; }

    public Size WorldSize { get; private set; }

    public long CurrentSpan { get; private set; }

    public RandomTable RandomTable { get; private set; } = new();

    public ArchiveInfo(string worldName, Size worldSize)
    {
        WorldName = worldName;
        LastSaveTime = CreateTime = DateTime.Now;
        Id = HashTool.ToMd5HashString(WorldName + CreateTime.ToBinary());
        WorldSize = worldSize;
        CurrentSpan = 0;
        RandomTable = new(1000);
        using var query = GetQuery();
        query.CreateTable<ArchiveInfo>(ARCHIVE_INFO);
        query.InsertItem(ARCHIVE_INFO, this);
    }

    public ArchiveInfo()
    {

    }

    public string GetDatabase()
    {
        return Path.Combine(RootPath, Id + ".db");
    }

    public SQLiteQuery GetQuery()
    {
        return new SQLiteQuery(GetDatabase());
    }

    public void UpdateLastSaveTime()
    {
        using var query = GetQuery();
        query.CreateTable<ArchiveInfo>(ARCHIVE_INFO);
        LastSaveTime = DateTime.Now;
        query.UpdateItem(ARCHIVE_INFO, this);
    }

    public void UpdateCurrentSpan(long currentSpan)
    {
        using var query = GetQuery();
        query.CreateTable<ArchiveInfo>(ARCHIVE_INFO);
        CurrentSpan = currentSpan;
        query.UpdateItem(ARCHIVE_INFO, this);
    }
}
