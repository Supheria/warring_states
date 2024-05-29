using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;

namespace WarringStates.Graph;

public partial class LatticeGrid : ISsSerializable
{
    public string LocalName => nameof(LatticeGrid);

    static GridData GridData { get; set; } = new();

    static CellData CellData { get; set; } = new();

    public static int CellEdgeLength
    {
        get => CellData.EdgeLength;
        set
        {
            CellData.EdgeLength = value;
            CellCenterPadding = (CellData.EdgeLength * CellData.CenterPaddingFactor).ToRoundInt();
            CellCenterSize = new(CellEdgeLength - CellCenterPadding * 2, CellEdgeLength - CellCenterPadding * 2);
            CellCenterSizeAddOnePadding = new(CellCenterSize.Width + CellCenterPadding, CellCenterSize.Height + CellCenterPadding);
        }
    }

    public static int CellCenterPadding { get; private set; } = (CellData.EdgeLength * CellData.CenterPaddingFactor).ToRoundInt();

    public static Size CellCenterSize { get; private set; } = new(CellEdgeLength - CellCenterPadding * 2, CellEdgeLength - CellCenterPadding * 2);

    public static Size CellCenterSizeAddOnePadding { get; private set; } = new(CellCenterSize.Width + CellCenterPadding, CellCenterSize.Height + CellCenterPadding);

    Rectangle DrawRect { get; set; }

    Image? Image { get; set; }

    Graphics? Graphics { get; set; }

    public Coordinate Origin { get; private set; } = new();

    public void EnableListner()
    {
        LocalEvents.Hub.AddListener<Coordinate>(LocalEvents.Graph.SetGridOrigin, SetOrigin);
        LocalEvents.Hub.AddListener<Coordinate>(LocalEvents.Graph.OffsetGridOrigin, OffsetOrigin);
        LocalEvents.Hub.AddListener<GridImageToUpdateArgs>(LocalEvents.Graph.GridImageToUpdate, UpdateImage);
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
