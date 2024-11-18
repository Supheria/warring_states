using AltitudeMapGenerator.DLA;
using AltitudeMapGenerator.VoronoiDiagram;
using LocalUtilities.TypeGeneral;

namespace AltitudeMapGenerator;

public class AltitudeMap
{
    public Rectangle Bounds { get; private set; } = new();

    public double AltitudeMax { get; private set; } = 0;

    public HashSet<Coordinate> OriginPoints { get; private set; } = [];

    public HashSet<Coordinate> RiverPoints { get; private set; } = [];

    public Dictionary<Coordinate, AltitudePoint> AltitudePoints { get; private set; } = [];

    public int Width => Bounds.Width;

    public int Height => Bounds.Height;

    public Size Size => Bounds.Size;

    public int Area => Bounds.Width * Bounds.Height;

    public AltitudeMap(AltitudeMapData data, IProgressor? progressor)
    {
        VoronoiPlane plane;
        List<Coordinate> sites;
        RiverGenerator river;
        do
        {
            Bounds = new(new(0, 0), data.Size);
            plane = new VoronoiPlane(data.Size);
            sites = plane.GenerateSites(data.SegmentNumber);
            river = new RiverGenerator(data.RiverWidth, data.Size, data.RiverSegmentNumber, data.RiverLayoutType, sites);
        } while (river.Successful is false);
        RiverPoints = river.River;

        progressor?.Reset(data.PixelNumber);
        DlaMap.Progressor = progressor;

        var pixels = new List<DlaPixel>();
        var altitudes = new List<double>();
        var origins = new List<Coordinate>();
        Parallel.ForEach(plane.Generate(sites), (cell) =>
        {
            var dlaMap = new DlaMap(cell);
            pixels.AddRange(dlaMap.Generate((int)(cell.Area / Area * data.PixelNumber), data.PixelDensity));
            altitudes.Add(dlaMap.AltitudeMax);
            origins.Add(cell.Site);
        });
        OriginPoints = origins.ToHashSet();
        foreach (var pixel in pixels)
        {
            var coordinate = new Coordinate(pixel.X, pixel.Y);
            if (AltitudePoints.TryGetValue(coordinate, out AltitudePoint? point))
            {
                point.Altitude += pixel.Altitude;
                altitudes.Add(point.Altitude);
            }
            else
                AltitudePoints[coordinate] = new(coordinate, pixel.Altitude);
        }
        AltitudeMax = altitudes.Max();
    }
}
