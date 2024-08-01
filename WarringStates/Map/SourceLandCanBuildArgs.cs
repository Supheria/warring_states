using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;
using WarringStates.Map;

namespace WarringStates.Map;

public class SourceLandCanBuildArgs(Coordinate site, SourceLandTypes[] canBuildTypes) : EventArgs
{
    public Coordinate Site { get; private set; } = site;

    public SourceLandTypes[] CanbuildTypes { get; private set; } = canBuildTypes;

    public SourceLandCanBuildArgs() : this(new(), [])
    {

    }
}
