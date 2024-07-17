namespace AltitudeMapGenerator.VoronoiDiagram;

internal class VoronoiException(string message) : Exception(message)
{
    internal static void ThrowIfCountZero<T>(List<T> items, string name)
    {
        if (items.Count is 0)
            throw new VoronoiException($"{name} is empty");
    }

    internal static VoronoiException EmptyMinHeap()
    {
        return new($"Min heap is empty");
    }

    internal static VoronoiException NullVertexOfBorderClosingNode()
    {
        return new($"vertex of node in border closing is null");
    }

    internal static VoronoiException NullFortuneCircleEvent(string name)
    {
        return new($"{name} is null fortune circle event");
    }

    internal static VoronoiException NullFortuneSiteEvent(string name)
    {
        return new($"{name} is null fortune circle event");
    }

    internal static VoronoiException NoMatchVertexInDijkstra(string name)
    {
        return new($"there is no match to {name} in vertex list of dijkstra");
    }
}
