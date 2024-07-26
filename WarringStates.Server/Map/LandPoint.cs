using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Server.Map;

public class LandPoint(Coordinate coordinate, double altitudeRatio, PointTypes type)
{
    [TableField(IsPrimaryKey = true)]
    public Coordinate Coordinate { get; private set; } = coordinate;

    public double AltitudeRatio { get; private set; } = altitudeRatio;

    public PointTypes Type { get; private set; } = type;

    public LandPoint() : this(new(), 0, PointTypes.Normal)
    {

    }
}
