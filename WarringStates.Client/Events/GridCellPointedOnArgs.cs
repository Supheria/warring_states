using LocalUtilities.TypeGeneral;
using WarringStates.Client.UI;

namespace WarringStates.Client.Events;

public sealed class GridCellPointedOnArgs(MouseOperates mouseOperate, Coordinate site, Directions realPointOnPart) : EventArgs
{
    public MouseOperates MouseOperate { get; set; } = mouseOperate;

    public Coordinate Site { get; } = site;

    public Directions PointOnCellPart { get; } = realPointOnPart;
}
