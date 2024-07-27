using LocalUtilities.FileHelper;
using LocalUtilities.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Server.User;
using WarringStates.User;

namespace WarringStates.Server;

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
