using LocalUtilities.FileHelper;
using LocalUtilities.General;
using LocalUtilities.SimpleScript;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Client.UI;

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

    public static Size GridSize { get; private set; } = new(AtlasEx.Width * CellEdgeLength, AtlasEx.Height * CellEdgeLength);

    public static Rectangle GridDrawRange { get; private set; } = new(-CellEdgeLength, -CellEdgeLength, GridSize.Width, GridSize.Height);

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

    public static void PointOnCell(Point realPoint, MouseOperates mouseOperate)
    {
        var cell = new Cell(realPoint);
        var sendArgs = new GridCellPointedOnArgs(mouseOperate, cell.Site, cell.PointOnPart);
        LocalEvents.TryBroadcast(LocalEvents.Graph.PointOnCell, sendArgs);
    }
}
