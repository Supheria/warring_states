using LocalUtilities;

namespace WarringStates.Map;

public class SourceLand(Coordinate site, SourceLandTypes type, Directions direction/*, List<Product> products*/) : Land
{
    public SourceLandTypes Type { get; set; } = type;

    public override Coordinate Site { get; set; } = site;

    public static ColorSelector Colors { get; } = new SourceLandColors();

    public override Color Color => Colors[Type];

    public Directions Direction { get; set; } = direction;

    public SourceLand() : this(new(), SourceLandTypes.None, Directions.None)
    {

    }

    public Coordinate[] GetAllSites()
    {
        var leftTop = GetLeftTopSite();
        var sites = new List<Coordinate>();
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                sites.Add(new(leftTop.X + i, leftTop.Y + j));
            }
        }
        return sites.ToArray();
    }

    private Coordinate GetLeftTopSite()
    {
        return Direction switch
        {
            Directions.Center => Site - (1, 1),
            Directions.Left => Site - (0, 1),
            Directions.Top => Site - (1, 0),
            Directions.Right => Site - (2, 1),
            Directions.Bottom => Site - (1, 2),
            Directions.LeftTop => Site,
            Directions.TopRight => Site - (2, 0),
            Directions.LeftBottom => Site - (0, 2),
            Directions.BottomRight => Site - (2, 2),
            _ => Site
        };
    }

    public Coordinate GetCenterSite()
    {
        return Direction switch
        {
            Directions.Left => Site + (1, 0),
            Directions.Top => Site + (0, 1),
            Directions.Right => Site - (1, 0),
            Directions.Bottom => Site - (0, 1),
            Directions.LeftTop => Site + (1, 1),
            Directions.TopRight => Site + (1, -1),
            Directions.LeftBottom => Site + (1, -1),
            Directions.BottomRight => Site - (1, 1),
            _ => Site
        };
    }
}
