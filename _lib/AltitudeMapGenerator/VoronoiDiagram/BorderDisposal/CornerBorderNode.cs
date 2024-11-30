using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal class CornerBorderNode(VoronoiVertex point) : BorderNode
{
    public override Directions BorderLocation { get; } = point.DirectionOnBorder;

    public override VoronoiVertex Vertex { get; } = point;

    public override double Angle => throw new InvalidOperationException();

    public override int FallbackComparisonIndex => throw new InvalidOperationException();
}
