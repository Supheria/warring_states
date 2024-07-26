using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class Player : IRosterItem<string>
{
    [TableField(IsPrimaryKey = true, Name = "PID")]
    public string Id { get; private set; }

    public string Name { get; private set; }

    public string Password { get; private set; }

    public string Signature => Id;

    public Player(string id, string name, string password)
    {
        Id = id;
        Name = name;
        Password = password;
    }

    public Player() : this("", "", "")
    {

    }

    public static string CreateId(string name)
    {
        return HashTool.ToMd5HashString(name + DateTime.Now.ToBinary());
    }

    public static string ConvertPasswortText(string passwordText)
    {
        return HashTool.ToMd5HashString(passwordText);
    }

    public static bool operator ==(Player? userInfo, object? obj)
    {
        if (userInfo is null)
        {
            if (obj is null)
                return true;
            else
                return false;
        }
        if (obj is not Player other)
            return false;
        return userInfo.Id == other.Id && userInfo.Password == other.Password;
    }

    public static bool operator !=(Player? userInfo, object? obj)
    {
        return !(userInfo == obj);
    }

    public override bool Equals(object? obj)
    {
        return this == obj;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
