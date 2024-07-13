using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Server.User;

internal class PlayerRoster : Roster<string, Player>
{
    public override string LocalName => nameof(PlayerRoster);
}
