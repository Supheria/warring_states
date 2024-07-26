using LocalUtilities.FileHelper;
using LocalUtilities.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Server;

internal class LocalDataBase
{
    public static string Path { get; } = "local.db";

    public static SQLiteQuery NewQuery()
    {
        return new SQLiteQuery(Path);
    }
}
