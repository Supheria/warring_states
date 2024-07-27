using LocalUtilities.SQLiteHelper;
using WarringStates.Server.User;
using WarringStates.User;

namespace WarringStates.Server.Data;

internal class LocalDataBase
{
    public static string Path { get; } = "local.db";

    public static string NameofPlayer { get; } = nameof(Player);

    public static string NameofArchive { get; } = nameof(Archive);

    public static SQLiteQuery NewQuery()
    {
        return new SQLiteQuery(Path);
    }
}
