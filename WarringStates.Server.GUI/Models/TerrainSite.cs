using LocalUtilities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Server.GUI.Models;

internal class TerrainSite(Coordinate site, SingleLandTypes landType)
{
    public Coordinate Site => new(X, Y);

    public int X { get; private set; } = site.X;

    public int Y { get; private set; } = site.Y;

    public SingleLandTypes Type { get; private set; } = landType;

    public TerrainSite() : this(new(), SingleLandTypes.None)
    {

    }
}
