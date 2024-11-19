using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeGeneral;
using System.Diagnostics.CodeAnalysis;
using WarringStates.Net.Common;
using WarringStates.Server.Data;
using WarringStates.User;

namespace WarringStates.Server.Net;

internal class LocalNet
{
    public static ServiceManager Server { get; } = new();

    public static int Port { get; set; } = 60;

    public static void Switch()
    {
        if (Server.IsStart)
            Server.Close();
        else
            Server.Start(Port);
    }

    public static bool CheckLogin(string name, string passwordText, [NotNullWhen(true)] out Player? player, out ServiceCode code)
    {
        player = null;
        using var query = LocalDataBase.NewQuery();
        query.CreateTable<Player>(LocalDataBase.PLAYER);
        var condition = new Condition(SQLiteQuery.GetFieldName<Player>(nameof(Player.Name)), name, Operators.Equal);
        var selects = query.SelectItems<Player>(LocalDataBase.PLAYER, condition);
        if (selects.Length > 1)
        {
            code = ServiceCode.MultiPlayerName;
            return false;
        }
        if (selects.Length is 0)
        {
            query.Commit();
            return AddPlayer(name, passwordText, out player, out code);
        }
        if (selects[0].Password == Player.ConvertPasswortText(passwordText))
        {
            player = selects[0];
            code = ServiceCode.Success;
            return true;
        }
        else
        {
            code = ServiceCode.WrongPassword;
            return false;
        }
    }

    public static bool AddPlayer(string name, string passwordText, [NotNullWhen(true)] out Player? player, out ServiceCode code)
    {
        player = null;
        using var query = LocalDataBase.NewQuery();
        query.CreateTable<Player>(LocalDataBase.PLAYER);
        var condition = new Condition(SQLiteQuery.GetFieldName<Player>(nameof(Player.Name)), name, Operators.Equal);
        var selects = query.SelectItems<Player>(LocalDataBase.PLAYER, condition);
        if (selects.Length > 0)
        {
            player = null;
            code = ServiceCode.MultiPlayerName;
            return false;
        }
        if (string.IsNullOrEmpty(name) || name.TrimStart().TrimEnd().Length is 0)
        {
            code = ServiceCode.EmptyPlayerName;
            return false;
        }
        if (string.IsNullOrEmpty(passwordText))
        {
            code = ServiceCode.EmptyPassword;
            return false;
        }
        player = new Player(name, Player.ConvertPasswortText(passwordText));
        query.InsertItem(LocalDataBase.PLAYER, player);
        code = ServiceCode.Success;
        return true;
    }
}
