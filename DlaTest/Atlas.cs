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

    Dictionary<Direction, List<VoronoiCell>> CellDirectionMap { get; } = new()
    {
        [Direction.Left] = [],
        [Direction.Top] = [],
        [Direction.Right] = [],
        [Direction.Bottom] = [],
    };

    public Dictionary<Coordinate, DlaPixel[]> PixelsMap { get; } = [];

    public List<Edge> River { get; } = [];

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
        Cells = VoronoiPlane.Generate(Width, Height, WidthSegmentNumber, HeightSegmentNumber, pointGeneration);
        GenerateRiver();
        foreach (var cell in Cells)
            PixelsMap[cell.Centroid] = DlaMap.Generate(cell, (int)(cell.GetArea() / area * TotalPixelNumber));
    }

    private void GenerateRiver()
    {
        CellDirectionMap[Direction.Left].Clear();
        CellDirectionMap[Direction.Top].Clear();
        CellDirectionMap[Direction.Right].Clear();
        CellDirectionMap[Direction.Bottom].Clear();
        HashSet<Coordinate> nodes = [];
        HashSet<Edge> edges = [];
        var riverOverlayType = Random.Next() % 2;
        foreach (var cell in Cells)
        {
            if (RiverOverlayFilter(cell.Centroid, cell.DirectionOnBorder, riverOverlayType, true))
            {
                if (cell.DirectionOnBorder.HasFlag(Direction.Left))
                    CellDirectionMap[Direction.Left].Add(cell);
                else if (cell.DirectionOnBorder.HasFlag(Direction.Right))
                    CellDirectionMap[Direction.Right].Add(cell);
            }
            if (RiverOverlayFilter(cell.Centroid, cell.DirectionOnBorder, riverOverlayType, false))
            {
                if (cell.DirectionOnBorder.HasFlag(Direction.Top))
                    CellDirectionMap[Direction.Top].Add(cell);
                else if (cell.DirectionOnBorder.HasFlag(Direction.Bottom))
                    CellDirectionMap[Direction.Bottom].Add(cell);
            }
            foreach (var vertex in cell.Vertexes)
            {
                if (vertex.DirectionOnBorder is Direction.None)
                    nodes.Add(vertex.Coordinate);
                var nextVertex = cell.VerticeClockwiseNext(vertex);
                edges.Add(new(vertex.Coordinate, nextVertex.Coordinate));
            }
        }
        var startVertical = GetRiverEndPoint(Direction.Top, riverOverlayType);
        var endVertical = GetRiverEndPoint(Direction.Bottom, riverOverlayType);
        var startHorizontal = GetRiverEndPoint(Direction.Left, riverOverlayType);
        var endHorizontal = GetRiverEndPoint(Direction.Right, riverOverlayType);
        nodes.Add(startVertical);
        nodes.Add(endVertical);
        nodes.Add(startHorizontal);
        nodes.Add(endHorizontal);
        Dijkstra.Initialize(edges.ToList(), nodes.ToList());
        River.AddRange(Dijkstra.GetPath(startVertical, endVertical));
        River.AddRange(Dijkstra.GetPath(startHorizontal, endHorizontal));
    }

    private Coordinate GetRiverEndPoint(Direction direction, int riverOverlayType)
    {
        var cells = CellDirectionMap[direction];
        var cell = cells[Random.Next(0, cells.Count)];
        var vertexes = new List<VoronoiVertex>();
        foreach(var vertex in cell.Vertexes)
        {
            if (vertex.DirectionOnBorder != direction)
                continue;
            if (RiverOverlayFilter(vertex.Coordinate, vertex.DirectionOnBorder, riverOverlayType, true) ||
                RiverOverlayFilter(vertex.Coordinate, vertex.DirectionOnBorder, riverOverlayType, false))
                vertexes.Add(vertex);
        }
        return vertexes[Random.Next(0, vertexes.Count)].Coordinate;

    }

    // [riverOverlayType is 0]  [riverOverlayType is 1)
    //         _______                 _______
    //        |     / |               | \     |
    //        |\      |               |      /|
    //        |      \|               |/      |
    //        | /     |               |     \ |
    //         -------                 -------
    private bool RiverOverlayFilter(Coordinate coordinate, Direction direction, int riverOverlayType, bool horizontal)
    {
        if (horizontal)
        {
            if (direction.HasFlag(Direction.Left))
            {
                if ((riverOverlayType is 0 && coordinate.Y.ApproxLessThan(HeightHalf)) ||
                    (riverOverlayType is 1 && coordinate.Y.ApproxGreaterThan(HeightHalf)))
                    return true;
                return false;
            }
            else if (direction.HasFlag(Direction.Right))
            {
                if ((riverOverlayType is 0 && coordinate.Y.ApproxGreaterThan(HeightHalf)) ||
                    (riverOverlayType is 1 && coordinate.Y.ApproxLessThan(HeightHalf)))
                    return true;
                return false;
            }
            return false;
        }
        else
        {
            if (direction.HasFlag(Direction.Top))
            {
                if ((riverOverlayType is 0 && coordinate.X.ApproxGreaterThan(WidthHalf)) ||
                    (riverOverlayType is 1 && coordinate.X.ApproxLessThan(WidthHalf)))
                    return true;
                return false;
            }
            else if (direction.HasFlag(Direction.Bottom))
            {
                if ((riverOverlayType is 0 && coordinate.X.ApproxLessThan(WidthHalf)) ||
                    (riverOverlayType is 1 && coordinate.X.ApproxGreaterThan(WidthHalf)))
                    return true;
                return false;
            }
            return false;
        }
    }

#if DEBUG
    [Obsolete("just for test")]
    public List<VoronoiCell> GenerateVoronoi(IPointsGeneration pointGeneration)
    {
        return VoronoiPlane.Generate(Width, Height, WidthSegmentNumber, HeightSegmentNumber, pointGeneration);
    }

    [Obsolete("just for test")]
    public List<Edge> GenerateRiver(List<VoronoiCell> cells)
    {
        Cells = cells;
        GenerateRiver();
        return River;
    }
#endif
}
