using LocalUtilities.TypeGeneral;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class Player : RosterItem<string>
{
    public UserInfo UserInfo { get; private set; } = new();

    public override string Signature => UserInfo.Id;
}
