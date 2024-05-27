using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.Graph;

public partial class LatticeGrid : ISsSerializable
{
    public string LocalName => nameof(LatticeGrid);

    public GridData GridData { get; set; } = new();

    public static CellData CellData { get; set; } = new();

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
        var latticeCell = RealPointToLatticePoint(realPoint);
        var cell = new Cell(latticeCell);
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.PointOnCell, new PointOnCellArgs(cell.TerrainPoint, cell.GetRealPointOnPart(realPoint)));
    }

    public Coordinate RealPointToLatticePoint(Point realPoint)
    {
        var dX = realPoint.X - Origin.X;
        var x = dX / CellData.EdgeLength;
        if (dX < 0)
            x--;
        var dY = realPoint.Y - Origin.Y;
        var y = dY / CellData.EdgeLength;
        if (dY < 0)
            y--;
        return new(x, y);
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteObject(GridData);
        serializer.WriteObject(CellData);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        deserializer.ReadObject(GridData);
        deserializer.ReadObject(CellData);
    }
}
