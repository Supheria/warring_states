using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class Player(string name, string password) : IRosterItem<string>
{
    [TableField(IsPrimaryKey = true, Name = "shit fuck it")]
    public string Id { get; private set; } = HashTool.ToMd5HashString(name + DateTime.Now.ToBinary());

    [TableField(Name = "shit the shit wha tfuch @@\' \" # sjf= -dsmsdf oiwee9\" ")]
    public string Name { get; set; } = name;

    public string Password { get; private set; } = HashTool.ToMd5HashString(password);

    public string Signature => Id;

    public Player() : this("admin", "password")
    {

    }

    public void SetPasspord(string passpord)
    {
        Password = HashTool.ToMd5HashString(password);
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
