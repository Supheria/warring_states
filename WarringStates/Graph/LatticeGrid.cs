using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.Graph;

public partial class LatticeGrid
{
    public GridData GridData { get; set; } = new GridData().LoadFromSimpleScript();

    Rectangle DrawRect { get; set; }

    Graphics? Graphics { get; set; }

    public Coordinate Origin { get; set; } = new();

    public void EnableListner()
    {
        LocalEvents.Hub.AddListener<GameImageUpdateArgs>(LocalEvents.Graph.GameImageUpdate, DrawGrid);
        LocalEvents.Hub.AddListener<Point>(LocalEvents.Graph.PointOnGameImage, GetLatticeCell);
    }

    public void GetLatticeCell(Point realPoint)
    {
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.PointOnCell, new LatticeCell(Origin, realPoint));
    }
}
