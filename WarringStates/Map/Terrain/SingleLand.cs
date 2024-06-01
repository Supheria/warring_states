using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Graph;
using WarringStates.Terrain;

namespace WarringStates.Map.Terrain;

public class SingleLand : ILand
{
    public enum Types
    {
        Plain,
        Stream,
        Wood,
        Hill,
    }

    public class SingleLandColors : ColorSelector
    {
        public override string LocalName => nameof(SingleLandColors);

        protected override Dictionary<Enum, Color> Colors { get; } = new()
        {
            [Types.Plain] = Color.LightYellow,
            [Types.Wood] = Color.LimeGreen,
            [Types.Stream] = Color.SkyBlue,
            [Types.Hill] = Color.DarkSlateGray,
        };
    }

    public Enum Type { get; }

    public static SingleLandColors Colors { get; set; } = new();

    public Color Color => Colors[Type];

    public Coordinate Point { get; }

    public SingleLand(Coordinate point, Types type) : base()
    {
        Type = type;
        Point = point;
    }

    public SingleLand(Coordinate point, double altitudeRatio, double random)
    {
        Type = AltitudeFilter(altitudeRatio, random);
        Point = point;
    }

    public int DrawCell(Graphics? g, LatticeGrid.Cell cell, Rectangle drawRect, Color backColor, ILand? lastLand)
    {
        if (g is null)
            return 0;
        if (Type.Equals(lastLand?.Type))
            return 0;
        var count = 0;
        if (lastLand is SourceLand)
        {
            if (cell.RealRect.CutRectInRange(drawRect, out var r))
            {
                g?.FillRectangle(new SolidBrush(backColor), r.Value);
                count++;
            }
        }
        if (cell.CenterRealRect.CutRectInRange(drawRect, out var rect))
        {
            g?.FillRectangle(new SolidBrush(Colors[Type]), rect.Value);
            count++;
        }
        return count;
    }

    private static Types AltitudeFilter(double altitudeRatio, double random)
    {
        if (altitudeRatio.ApproxLessThan(0.05))
        {
            if (random.ApproxLessThan(0.33))
                return Types.Plain;
            if (random.ApproxLessThan(0.9))
                return Types.Wood;
        }
        else if (altitudeRatio.ApproxLessThan(0.15))
        {
            if (random.ApproxLessThan(0.33))
                return Types.Wood;
            if (random.ApproxLessThan(0.95))
                return Types.Hill;
        }
        else
        {
            if (random.ApproxLessThan(0.8))
                return Types.Hill;
            if (random.ApproxLessThan(0.99))
                return Types.Wood;
        }
        return Types.Stream;
    }
}
