using AltitudeMapGenerator.DijkstraShortestPath;
using AltitudeMapGenerator.Layout;
using AltitudeMapGenerator.VoronoiDiagram;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities;
using LocalUtilities.General;

namespace AltitudeMapGenerator;

internal class RiverGenerator
{
    RiverLayout RiverLayout { get; }

    Random Random { get; } = new();

    /// <summary>
    /// position of border node
    /// </summary>
    private enum NodeBorderPosition
    {
        LeftOrTop = 0,
        RightOrBottom,
    }

    /// <summary>
    /// nodes on the border
    /// </summary>
    Dictionary<(int, NodeBorderPosition Towards), HashSet<Coordinate>> BorderNodeMap { get; } = [];

    HashSet<Coordinate> InnerNodes { get; set; } = [];

    HashSet<Edge> Edges { get; set; } = [];

    double EdgeLengthMax { get; set; } = 0;

    HashSet<Edge> Rivers { get; set; } = [];

    Dictionary<int, List<Coordinate>> EdgeSiteMap { get; } = [];

    double Width { get; }

    internal HashSet<Coordinate> River { get; private set; } = [];

    /// <summary>
    /// when rivers generated not match to layout, will be set to false in <see cref="GenerateRiver"/>
    /// </summary>
    internal bool Successful { get; private set; } = true;

    internal RiverGenerator(double width, Size size, Size segmentNumber, RiverLayout.Types riverLayoutType, List<Coordinate> existedSites)
    {
        Width = width;
        RiverLayout = riverLayoutType.Parse()(size);
        List<VoronoiCell> cells;
        var plane = new VoronoiPlane(size);
        var sites = plane.GenerateSites(segmentNumber, existedSites);
        cells = plane.Generate(sites);
        foreach (var cell in cells)
        {
            for (int i = 0; i < cell.Vertexes.Count; i++)
            {
                var vertex = cell.Vertexes[i];
                if (vertex.DirectionOnBorder is Directions.None)
                    InnerNodes.Add(vertex);
                else
                    BorderNodeFilter(vertex);
                var nextVertex = cell.VertexCounterClockwiseNext(vertex);
                addEdge(new(vertex, nextVertex), cell.Site);
            }
        }
        for (int i = 0; i < RiverLayout.Layout.Count; i++)
            GenerateRiver(i);
        GenerateBranch();
        void addEdge(Edge edge, Coordinate site)
        {
            Edges.Add(edge);
            var signature = GetEdgeSignature(edge);
            if (EdgeSiteMap.TryGetValue(signature, out var list))
                list.Add(site);
            else
                EdgeSiteMap[signature] = [site];
            EdgeLengthMax = Math.Max(edge.Length, EdgeLengthMax);
        }
    }

    private int GetEdgeSignature(Edge edge)
    {
        //return HashCode.Combine(Starter.GetHashCode(), Ender.GetHashCode());
        var left = edge.Starter.X < edge.Ender.X ? edge.Starter : edge.Ender;
        var right = edge.Starter.X > edge.Ender.X ? edge.Starter : edge.Ender;
        return HashCode.Combine(left, right);
    }

    /// <summary>
    /// select border nodes fit to type of river layout
    /// </summary>
    /// <param name="vertex"></param>
    private void BorderNodeFilter(VoronoiVertex vertex)
    {
        for (int i = 0; i < RiverLayout.Layout.Count; i++)
        {
            if (RiverLayout.Layout[i].Start.VoronoiVertexFilter(vertex))
            {
                var key = (i, NodeBorderPosition.LeftOrTop);
                if (BorderNodeMap.TryGetValue(key, out var nodes))
                    nodes.Add(vertex);
                else
                    BorderNodeMap[key] = [vertex];
            }
            else if (RiverLayout.Layout[i].Finish.VoronoiVertexFilter(vertex))
            {
                var key = (i, NodeBorderPosition.RightOrBottom);
                if (BorderNodeMap.TryGetValue(key, out var nodes))
                    nodes.Add(vertex);
                else
                    BorderNodeMap[key] = [vertex];
            }
        }
    }

    private void GenerateRiver(int riverIndex)
    {
        List<Coordinate> startNodes, finishNodes;
        if (BorderNodeMap.TryGetValue((riverIndex, NodeBorderPosition.LeftOrTop), out var set))
            startNodes = set.ToList();
        else
        {
            Successful = false;
            return;
        }
        if (BorderNodeMap.TryGetValue((riverIndex, NodeBorderPosition.RightOrBottom), out set))
            finishNodes = set.ToList();
        else
        {
            Successful = false;
            return;
        }
        List<Edge>? river = null;
        var startVisited = new HashSet<Coordinate>();
        var finishVisited = new HashSet<Coordinate>();
        var existed = Rivers.ToHashSet();
        do
        {
            if (startVisited.Count == startNodes.Count && finishNodes.Count == finishNodes.Count)
                break;
            var start = startNodes[Random.Next(0, startNodes.Count)];
            var finish = finishNodes[Random.Next(0, finishNodes.Count)];
            startVisited.Add(start);
            finishVisited.Add(finish);
            var nodes = InnerNodes.ToList();
            nodes.AddRange([start, finish]);
            river = new Dijkstra(Edges.ToList(), nodes, start, finish).Path;
        } while (river is null || river.FirstOrDefault(existed.Contains) is not null);
        if (river is not null && river.FirstOrDefault(existed.Contains) is null)
            river.ForEach(e => Rivers.Add(e));
        else
            Successful = false;
    }

    private void GenerateBranch()
    {
        var visited = new HashSet<Coordinate>();
        foreach (var edge in Rivers)
        {
            var points = edge.GetInnerPoints(Width);
            points.ForEach(x => River.Add(x));
            if (edge.Length.ApproxLessThan(EdgeLengthMax / 5))
                continue;
            var point = points[Random.Next(0, points.Count)];
            var sites = EdgeSiteMap[GetEdgeSignature(edge)];
            var site = sites[Random.Next(0, sites.Count)];
            if (visited.Contains(site))
                continue;
            visited.Add(site);
            new Edge(point, site).GetInnerPoints(Width / 2).ForEach(x => River.Add(x));
        }
    }
}
