using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class Player(string name, string password) : RosterItem<string>
{
    public string Name { get; private set; } = name;

    public string Id => Name.ToMd5HashString();

    public string Password { get; private set; } = password.ToMd5HashString();

    public override string Signature => Id;

    public Player() : this("", "")
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
