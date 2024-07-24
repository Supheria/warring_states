using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.User;

namespace WarringStates.Server.User;

public class Players : Roster<string, Player>
{
    //public int GetPlayerCount(ArchiveInfo archiveInfo)
    //{
    //    try
    //    {
    //        using var query = new SQLiteQuery(LocalArchives.GetPlayersPath(archiveInfo));
    //    }
    //    catch
    //    {
    //        return 0;
    //    }
    //}

    //public int InsertRange(Player[] players, string filePath)
    //{
    //    using var query = new SQLiteQuery(filePath);

    //}
}
