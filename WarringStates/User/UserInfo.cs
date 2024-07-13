using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class UserInfo(string name, string password) : ISsSerializable
{
    public string Name { get; private set; } = name;

    public string Id { get; private set; } = name.ToMd5HashString();

    public string Password { get; private set; } = password.ToMd5HashString();

    public string LocalName => nameof(UserInfo);

    public UserInfo() : this("", "")
    {

    }

    public static bool operator ==(UserInfo? userInfo, object? obj)
    {
        if (userInfo is null)
        {
            if (obj is null)
                return true;
            else
                return false;
        }
        if (obj is not UserInfo other)
            return false;
        return userInfo.Name == other.Name && userInfo.Password == other.Password;
    }

    public static bool operator !=(UserInfo? userInfo, object? obj)
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

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Name), Name);
        serializer.WriteTag(nameof(Password), Password);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Name = deserializer.ReadTag(nameof(Name));
        Password = deserializer.ReadTag(nameof(Password));
        Id = Name.ToMd5HashString();
    }
}
