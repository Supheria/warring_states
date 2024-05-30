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
        get => _cellEdgeLength;
        set
        {
            _cellEdgeLength = value < CellData.EdgeLengthMin || value > CellData.EdgeLengthMax ? _cellEdgeLength : value;
            CellCenterPadding = (_cellEdgeLength * CellData.CenterPaddingFactor).ToRoundInt();
            CellCenterSize = new(CellEdgeLength - CellCenterPadding * 2, CellEdgeLength - CellCenterPadding * 2);
            CellCenterSizeAddOnePadding = new(CellCenterSize.Width + CellCenterPadding, CellCenterSize.Height + CellCenterPadding);
        }
    }
    static int _cellEdgeLength = 30;

    public static int CellCenterPadding { get; private set; } = (CellEdgeLength * CellData.CenterPaddingFactor).ToRoundInt();

    public static Size CellCenterSize { get; private set; } = new(CellEdgeLength - CellCenterPadding * 2, CellEdgeLength - CellCenterPadding * 2);

    public static Size CellCenterSizeAddOnePadding { get; private set; } = new(CellCenterSize.Width + CellCenterPadding, CellCenterSize.Height + CellCenterPadding);

    Rectangle DrawRect { get; set; }

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
        var x = dX / CellEdgeLength;
        if (dX < 0)
            x--;
        var dY = realPoint.Y - Origin.Y;
        var y = dY / CellEdgeLength;
        if (dY < 0)
            y--;
        return new(x, y);
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Origin), Origin.ToString());
        serializer.WriteObject(GridData);
        serializer.WriteObject(CellData);
        serializer.WriteTag(nameof(CellEdgeLength), CellEdgeLength.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Origin = deserializer.ReadTag(nameof(Origin), Coordinate.Parse);
        OriginOffset = Origin;
        deserializer.ReadObject(GridData);
        deserializer.ReadObject(CellData);
        CellEdgeLength = deserializer.ReadTag(nameof(CellEdgeLength), int.Parse);
    }
}
