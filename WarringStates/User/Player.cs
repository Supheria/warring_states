using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.User;

public class Player
{
    public string ArchiveId { get; }

    public string Name { get; }

    public HashSet<Coordinate> SouceLandPoints { get; } = [];

    public Coordinate GridOrigin { get; private set; } = new();
}
