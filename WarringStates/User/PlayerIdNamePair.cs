using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.User;

public class PlayerIdNamePair(string id, string name)
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    private PlayerIdNamePair() : this("", "")
    {

    }

    public PlayerIdNamePair(Player player) : this(player.Id, player.Name)
    {

    }
}
