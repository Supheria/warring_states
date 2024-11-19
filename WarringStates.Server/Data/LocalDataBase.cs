using LocalUtilities.SQLiteHelper;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Server.Data;

internal class LocalDataBase
{
    public static string Path { get; } = "local.db";

    public static string PLAYER { get; } = nameof(Player);

    public static string ARCHIVE_INFO { get; } = nameof(ArchiveInfo);

    public static SQLiteQuery NewQuery()
    {
        return new SQLiteQuery(Path);
    }
}
