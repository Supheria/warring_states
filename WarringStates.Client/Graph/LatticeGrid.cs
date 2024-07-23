using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;

namespace WarringStates.Client.Graph;

public partial class GridDrawer
{
    static GridData GridData { get; set; } = new();

    static CellData CellData { get; set; } = new();

    public GridDrawer(GridData gridData, CellData cellData)
    {
        GridData = gridData;
        CellData = cellData;
        CellEdgeLength = cellData.EdgeLength;
        EnableListner();
    }

    public GridDrawer()
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
        LocalEvents.TryAddListener<GridOriginOperateArgs>(LocalEvents.Graph.OperateGridOrigin, OperateOrigin);
        LocalEvents.TryAddListener<GridToRelocateArgs>(LocalEvents.Graph.GridToRelocate, Relocate);
        LocalEvents.TryAddListener<PointOnGridCellArgs>(LocalEvents.Graph.PointOnGridCell, ConvertToCell);
    }

    private void ConvertToCell(PointOnGridCellArgs args)
    {
        var latticeCell = RealPointToLatticePoint(args.RealPoint);
        var cell = new Cell(latticeCell);
        var sendArgs = new GridCellPointedOnArgs(cell.TerrainPoint, cell.GetRealPointOnPart(args.RealPoint));
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridCellFromPoint, sendArgs);
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
