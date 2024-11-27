using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Server.Map;

internal class VisibleSite(Coordinate site, SourceLandTypes sourceType, Directions direction) : IRosterItem<Coordinate>
{
    [TableField(IsPrimaryKey = true)]
    public Coordinate Site { get; set; } = site;

    public SourceLandTypes SourceType { get; set; } = sourceType;

    public Directions Direction { get; set; } = direction;

    public Coordinate Signature => Site;

    public VisibleSite() : this(new(), SourceLandTypes.None, Directions.None)
    {

    }

    public VisibleSite(Coordinate site) : this(site, SourceLandTypes.None, Directions.None)
    {

    }

    public Coordinate GetCenter()
    {
        return Direction switch
        {
            Directions.Left => Site + (1, 0),
            Directions.Top => Site + (0, 1),
            Directions.Right => Site + (-1, 0),
            Directions.Bottom => Site + (0, -1),
            Directions.LeftTop => Site + (1, 1),
            Directions.TopRight => Site + (1, -1),
            Directions.LeftBottom => Site + (1, -1),
            Directions.BottomRight => Site + (-1, -1),
            _ => Site
        };
    }
}
