using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalUtilities;
using LocalUtilities.DijkstraShortestPath;
using LocalUtilities.GdiUtilities;
using LocalUtilities.Interface;
using LocalUtilities.VoronoiDiagram;
using LocalUtilities.VoronoiDiagram.Model;

namespace DlaTest;

public class Atlas(int width, int height, int widthSegmentNumber, int heightSegmentNumber, int pixelNumber)
{
    List<VoronoiCell> Cells { get; set; } = [];

    public Dictionary<Coordinate, DlaPixel[]> PixelsMap { get; } = [];

    List<Dictionary<Direction, List<VoronoiCell>>> CellMapForRiver { get; } = [[], []];

    public List<Edge> Rivers { get; } = [];

    // [riverOverlayType is 0]  [riverOverlayType is 1)
    //         _______                 _______
    //        | _____ |               | |   | |
    //        |       |               | |   | |
    //        |       |               | |   | |
    //        | ----- |               | |   | |
    //         -------                 -------
    int RiverOverlayType { get; set; } = 0;

    public int Width { get; } = width;

    public int Height { get; } = height;

    double WidthHalf { get; } = (double)width / 2f;

    double HeightHalf { get; } = (double)height / 2f;

    public Rectangle Bounds => new(0, 0, (int)Width, (int)Height);

    int WidthSegmentNumber { get; } = widthSegmentNumber;

    int HeightSegmentNumber { get; } = heightSegmentNumber;

    public int TotalPixelNumber { get; } = pixelNumber;

    Random Random { get; } = new();

    public Atlas() : this(0, 0, 0, 0, 0)
    {

    }

    public void Generate(IPointsGeneration pointGeneration)
    {
        long area = Width * Height;
        Cells = new VoronoiPlane(Width, Height).Generate(WidthSegmentNumber, HeightSegmentNumber, pointGeneration);
        GenerateRiver();
        foreach (var cell in Cells)
            PixelsMap[cell.Centroid] = new DlaMap(cell).Generate((int)(cell.GetArea() / area * TotalPixelNumber));
    }

    private void GenerateRiver()
    {
        RiverOverlayType = Random.Next() % 2;
        if (RiverOverlayType is 0)
            CellMapForRiver.ForEach(m => { m.Clear(); m.Add(Direction.Left, []); m.Add(Direction.Right, []); });
        else
            CellMapForRiver.ForEach(m => { m.Clear(); m.Add(Direction.Top, []); m.Add(Direction.Bottom, []); });
        HashSet<Coordinate> nodes = [];
        HashSet<Edge> edges = [];
        foreach (var cell in Cells)
        {
            if (RiverOverlayType is 0)
            {
                var index = cell.Centroid.Y.ApproxLessThanOrEqualTo(HeightHalf) ? 0 : 1;
                if (cell.DirectionOnBorder.HasFlag(Direction.Left))
                    CellMapForRiver[index][Direction.Left].Add(cell);
                else if (cell.DirectionOnBorder.HasFlag(Direction.Right))
                    CellMapForRiver[index][Direction.Right].Add(cell);
            }
            else
            {
                var index = cell.Centroid.X.ApproxLessThanOrEqualTo(WidthHalf) ? 0 : 1;
                if (cell.DirectionOnBorder.HasFlag(Direction.Top))
                    CellMapForRiver[index][Direction.Top].Add(cell);
                else if (cell.DirectionOnBorder.HasFlag(Direction.Bottom))
                    CellMapForRiver[index][Direction.Bottom].Add(cell);
            }
            foreach (var vertex in cell.Vertexes)
            {
                if (vertex.DirectionOnBorder is Direction.None)
                    nodes.Add(vertex.Coordinate);
                var nextVertex = cell.VerticeClockwiseNext(vertex);
                edges.Add(new(vertex.Coordinate, nextVertex.Coordinate));
            }
        }
        var start1st = GetRiverEndpoint(true, true);
        var end1st = GetRiverEndpoint(true, false);
        var start2nd = GetRiverEndpoint(false, true);
        var end2nd = GetRiverEndpoint(false, false);
        nodes.Add(start1st);
        nodes.Add(end1st);
        nodes.Add(start2nd);
        nodes.Add(end2nd);
        Dijkstra.Initialize(edges.ToList(), nodes.ToList());
        Rivers.AddRange(Dijkstra.GetPath(start1st, end1st));
        Rivers.AddRange(Dijkstra.GetPath(start2nd, end2nd));
    }

    private Coordinate GetRiverEndpoint(bool firstRiver, bool startEndpoint)
    {
        var direction = RiverOverlayType is 0 ? 
            startEndpoint ? Direction.Left : Direction.Right : 
            startEndpoint ? Direction.Top : Direction.Bottom;
        var cells = CellMapForRiver[firstRiver ? 0 : 1][direction];
        var vertexes = new List<VoronoiVertex>();
        do
        {
            var cell = cells[Random.Next(0, cells.Count)];
            foreach (var vertex in cell.Vertexes)
            {
                if (vertex.DirectionOnBorder != direction)
                    continue;
                if (RiverOverlayType is 0)
                {
                    if (firstRiver && vertex.Y.ApproxLessThanOrEqualTo(HeightHalf))
                        vertexes.Add(vertex);
                    else if ((!firstRiver) && vertex.Y.ApproxGreaterThanOrEqualTo(HeightHalf))
                        vertexes.Add(vertex);
                }
                else
                {
                    if (firstRiver && vertex.X.ApproxLessThanOrEqualTo(WidthHalf))
                        vertexes.Add(vertex);
                    else if ((!firstRiver) && vertex.X.ApproxGreaterThanOrEqualTo(WidthHalf))
                        vertexes.Add(vertex);
                }
            }
        } while (vertexes.Count is 0);
        return vertexes[Random.Next(0, vertexes.Count)].Coordinate;
    }

#if DEBUG
    [Obsolete("just for test")]
    public List<VoronoiCell> GenerateVoronoi(IPointsGeneration pointGeneration)
    {
        return new VoronoiPlane(Width, Height).Generate(WidthSegmentNumber, HeightSegmentNumber, pointGeneration);
    }

    [Obsolete("just for test")]
    public List<Edge> GenerateRiver(List<VoronoiCell> cells)
    {
        Cells = cells;
        GenerateRiver();
        return Rivers;
    }
#endif
}
