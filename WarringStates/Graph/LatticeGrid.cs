using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;

namespace WarringStates.Graph;

public partial class LatticeGrid
{
    static GridData GridData { get; set; } = new();

    static CellData CellData { get; set; } = new();

    public LatticeGrid(GridData gridData, CellData cellData)
    {
        GridData = gridData;
        CellData = cellData;
        CellEdgeLength = cellData.EdgeLength;
        EnableListner();
    }

    public LatticeGrid()
    {
        EnableListner();
    }

    public static int CellEdgeLength
    {
        get => CellData.EdgeLength;
        set
        {
            CellData.EdgeLength = value;
            CellCenterPadding = (CellData.EdgeLength * CellData.CenterPaddingFactor).ToRoundInt();
            CellCenterSize = new(CellData.EdgeLength - CellCenterPadding * 2, CellData.EdgeLength - CellCenterPadding * 2);
            CellCenterSizeAddOnePadding = new(CellCenterSize.Width + CellCenterPadding, CellCenterSize.Height + CellCenterPadding);
        }
    }

    public static int CellCenterPadding { get; private set; } = (CellEdgeLength * CellData.CenterPaddingFactor).ToRoundInt();

    public static Size CellCenterSize { get; private set; } = new(CellEdgeLength - CellCenterPadding * 2, CellEdgeLength - CellCenterPadding * 2);

    public static Size CellCenterSizeAddOnePadding { get; private set; } = new(CellCenterSize.Width + CellCenterPadding, CellCenterSize.Height + CellCenterPadding);

    Rectangle DrawRect { get; set; }

    public Coordinate Origin { get; private set; } = new();

    private void EnableListner()
    {
        LocalEvents.Hub.AddListener<Coordinate>(LocalEvents.Graph.GridOriginToReset, SetOrigin);
        LocalEvents.Hub.AddListener<Coordinate>(LocalEvents.Graph.GridOriginToOffset, OffsetOrigin);
        LocalEvents.Hub.AddListener<GridToRelocateArgs>(LocalEvents.Graph.GridToRelocate, Relocate);
        LocalEvents.Hub.AddListener<Point>(LocalEvents.Graph.GridCellToPointOn, GetLatticeCell);
    }

    public void GetLatticeCell(Point realPoint)
    {
        var latticeCell = RealPointToLatticePoint(realPoint);
        var cell = new Cell(latticeCell);
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridCellPointedOn, new GridCellPointedOnArgs(cell.TerrainPoint, cell.GetRealPointOnPart(realPoint)));
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

    //public void Serialize(SsSerializer serializer)
    //{
    //    serializer.WriteTag(nameof(Origin), Origin.ToString());
    //    serializer.WriteObject(GridData);
    //    serializer.WriteObject(CellData);
    //    serializer.WriteTag(nameof(CellEdgeLength), CellEdgeLength.ToString());
    //}

    //public void Deserialize(SsDeserializer deserializer)
    //{
    //    Origin = deserializer.ReadTag(nameof(Origin), Coordinate.Parse);
    //    OriginOffset = Origin;
    //    deserializer.ReadObject(GridData);
    //    deserializer.ReadObject(CellData);
    //    CellEdgeLength = deserializer.ReadTag(nameof(CellEdgeLength), int.Parse);
    //}
}
