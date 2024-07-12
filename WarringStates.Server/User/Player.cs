using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Map.Terrain;
using WarringStates.Net.Common;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class Player : ISsSerializable
{
    public UserInfo UserInfo { get; private set; } = new();

    public string LocalName => nameof(Player);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteObject(UserInfo);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        deserializer.ReadObject(UserInfo);
    }
}
