using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class Player(string name, string password) : IRosterItem<string>
{
    [TableField]
    public string Id { get; private set; } = (name + DateTime.Now.ToBinary()).ToMd5HashString();

    public string Name { get; private set; } = name;

    public string Password { get; private set; } = password.ToMd5HashString();

    public string Signature => Id;

    public Player() : this("admin", "password")
    {

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
        return userInfo.Name == other.Name && userInfo.Password == other.Password;
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
