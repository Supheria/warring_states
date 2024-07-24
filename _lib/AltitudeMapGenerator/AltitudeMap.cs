using AltitudeMapGenerator.DLA;
using AltitudeMapGenerator.VoronoiDiagram;
using LocalUtilities.TypeGeneral;

namespace AltitudeMapGenerator;

public class AltitudeMap
{
    public Rectangle Bounds { get; private set; } = new();

    public double AltitudeMax { get; private set; } = 0;

    public List<Coordinate> OriginPoints { get; private set; } = [];

    public List<Coordinate> RiverPoints { get; private set; } = [];

    public List<AltitudePoint> AltitudePoints { get; private set; } = [];

    //public double[] RandomTable { get; private set; } = [];

    public int Width => Bounds.Width;

    public int Height => Bounds.Height;

    public int Area => Bounds.Width * Bounds.Height;

    public AltitudeMap()
    {

    }

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
        RiverPoints = river.River.ToList();

        progressor?.Reset(data.PixelNumber);
        DlaMap.Progressor = progressor;

        var altitudes = new List<double>();
        Parallel.ForEach(plane.Generate(sites), (cell) =>
        {
            var dlaMap = new DlaMap(cell);
            var pixels = dlaMap.Generate((int)(cell.Area / Area * data.PixelNumber), data.PixelDensity);
            altitudes.Add(dlaMap.AltitudeMax);
            OriginPoints.Add(cell.Site);
            AltitudePoints.AddRange(pixels.Select(p => new AltitudePoint(p.X, p.Y, p.Altitude)));
        });
        var pointAltitudeMap = new Dictionary<Coordinate, double>();
        foreach (var point in AltitudePoints)
        {
            if (pointAltitudeMap.ContainsKey(point))
                pointAltitudeMap[point] += point.Altitude;
            else
                pointAltitudeMap[point] = point.Altitude;
        }
        AltitudePoints = pointAltitudeMap.Select(p => new AltitudePoint(p.Key.X, p.Key.Y, p.Value)).ToList();
        AltitudeMax = altitudes.Max();
        var random = new Random();
        //RandomTable = new double[1000];
        //for (int i = 0; i < RandomTable.Length; i++)
        //    RandomTable[i] = Math.Round(random.NextDouble(), 3);
    }

    //public void Serialize(SsSerializer serializer)
    //{
    //    serializer.WriteTag(nameof(Width), Width.ToString());
    //    serializer.WriteTag(nameof(Height), Height.ToString());
    //    serializer.WriteTag(nameof(AltitudeMax), AltitudeMax.ToString());
    //    serializer.WriteValues(nameof(OriginPoints), OriginPoints, c => c.ToString());
    //    serializer.WriteValues(nameof(RiverPoints), RiverPoints, c => c.ToString());
    //    serializer.WriteValues(nameof(AltitudePoints), AltitudePoints, p => p.ToString());
    //    serializer.WriteValues(nameof(RandomTable), RandomTable.ToList(), d => d.ToString());
    //}

    //public void Deserialize(SsDeserializer deserializer)
    //{
    //    var width = deserializer.ReadTag(nameof(Width), int.Parse);
    //    var height = deserializer.ReadTag(nameof(Height), int.Parse);
    //    Bounds = new(0, 0, width, height);
    //    AltitudeMax = deserializer.ReadTag(nameof(AltitudeMax), double.Parse);
    //    OriginPoints = deserializer.ReadValues(nameof(OriginPoints), Coordinate.Parse);
    //    RiverPoints = deserializer.ReadValues(nameof(RiverPoints), Coordinate.Parse);
    //    AltitudePoints = deserializer.ReadValues(nameof(AltitudePoints), AltitudePoint.Parse);
    //    RandomTable = deserializer.ReadValues(nameof(RandomTable), double.Parse).ToArray();
    //}
}
