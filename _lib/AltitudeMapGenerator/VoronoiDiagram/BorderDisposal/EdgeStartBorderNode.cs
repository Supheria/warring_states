using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal class EdgeStartBorderNode(VoronoiEdge edge, int fallbackComparisonIndex) :
        EdgeBorderNode(edge, fallbackComparisonIndex)
{
    public override Directions BorderLocation => Edge.Starter.DirectionOnBorder;

    public override VoronoiVertex Vertex => Edge.Starter;

    public override double Angle => Vertex.AngleTo(Edge.Ender); // away from border
}