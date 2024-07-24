using LocalUtilities.FileHelper;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;

namespace WarringStates.Client.Graph;

public partial class GridDrawer : IInitializeable
{
    static SsSignTable SignTable { get; } = new();

    static DataCollect Data { get; set; } = LoadData();

    static GridData GridData => Data.GridData;

    static CellData CellData => Data.CellData;

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

    public static Coordinate Origin { get; private set; } = new();

    public string InitializeName => nameof(GridDrawer);

    public string IniFileExtension => ".data";

    class DataCollect
    {
        public GridData GridData { get; set; } = new();

        public CellData CellData { get; set; } = new();
    }

    private static DataCollect LoadData()
    {
        var filePath = new GridDrawer().GetInitializeFilePath();
        DataCollect? data;
        try
        {
            data = SerializeTool.DeserializeFile<DataCollect>(new(), SignTable, filePath);
            if (data is not null)
                return data;
        }
        catch { }
        data = new();
        SerializeTool.SerializeFile(data, new(), SignTable, true, filePath);
        return data;
    }

    public static void PointOnCell(Point realPoint)
    {
        var latticeCell = RealPointToLatticePoint(realPoint);
        var cell = new Cell(latticeCell);
        var sendArgs = new GridCellPointedOnArgs(cell.TerrainPoint, cell.GetRealPointOnPart(realPoint));
        LocalEvents.TryBroadcast(LocalEvents.Graph.PointOnCell, sendArgs);
    }

    private static Coordinate RealPointToLatticePoint(Point realPoint)
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
