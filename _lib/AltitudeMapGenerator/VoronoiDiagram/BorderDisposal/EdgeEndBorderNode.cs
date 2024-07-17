using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.TypeGeneral;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal class EdgeEndBorderNode(VoronoiEdge edge, int fallbackComparisonIndex) :
        EdgeBorderNode(edge, fallbackComparisonIndex)
{
    public override Directions BorderLocation => Edge.Ender.DirectionOnBorder;

    public override VoronoiVertex Vertex => Edge.Ender;

    public override double Angle => Vertex.AngleTo(Edge.Starter); // away from border
}