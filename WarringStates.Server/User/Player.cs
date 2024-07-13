using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class Player : RosterItem<string>
{
    public UserInfo UserInfo { get; private set; } = new();

    public override string LocalName => nameof(Player);

    public override string Signature => UserInfo.Id;

    public override void Serialize(SsSerializer serializer)
    {
        serializer.WriteObject(UserInfo);
    }

    public override void Deserialize(SsDeserializer deserializer)
    {
        deserializer.ReadObject(UserInfo);
    }
}
