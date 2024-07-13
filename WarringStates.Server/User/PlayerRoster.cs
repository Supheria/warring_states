using LocalUtilities.TypeGeneral;

namespace WarringStates.Server.User;

internal class PlayerRoster : Roster<string, Player>
{
    public override string LocalName => nameof(PlayerRoster);
}
