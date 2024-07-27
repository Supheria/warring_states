using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.Net.Common;
using WarringStates.User;

namespace WarringStates.Server.Data;

internal class PlayerTable
{
    public static string Name { get; } = nameof(Player);

    public static string CheckoutPlayerId(string playerName)
    {
        using var query = LocalDataBase.NewQuery();
        var player = new Player() { Name = playerName };
        var select = query.SelectItems<Player>(Name, SQLiteQuery.GetCondition(player, Operators.Equal, nameof(player.Name)));
        if (select.Length > 1)
            throw new DatabaseException(ServiceCode.MultiPlayerName);
        if (select.Length is 0)
            throw new DatabaseException(ServiceCode.PlayerNotExist);
        return select[0].Id;
    }
}
