using LocalUtilities;

namespace AltitudeMapGenerator.DijkstraShortestPath;

internal class DijkstraNode(Coordinate node, int index)
{
    internal bool Used { get; set; } = false;
    internal List<Coordinate> Nodes { get; } = [];

    internal Coordinate Coordinate { get; set; } = node;

    internal int Index { get; set; } = index;

    internal double Weight { get; set; } = double.MaxValue;
}
