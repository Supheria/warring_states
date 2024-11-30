using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using LocalUtilities;

namespace AltitudeMapGenerator.Layout;

public class RiverEndnode(Directions direction, Operators operatorType, Size size)
{
    internal Directions Direction { get; } = direction;

    internal Operators OperatorType { get; } = operatorType;

    internal double CompareValue { get; } = direction switch
    {
        Directions.Left or Directions.Right => size.Height / 2d,
        Directions.Top or Directions.Bottom => size.Width / 2d,
        _ => throw AltitudeMapGeneratorException.NotProperRiverEndnodeDirection(direction)
    };

    internal bool VoronoiVertexFilter(VoronoiVertex vertex)
    {
        if (vertex.DirectionOnBorder != Direction)
            return false;
        var value = Direction switch
        {
            Directions.Left or Directions.Right => vertex.Y,
            Directions.Top or Directions.Bottom => vertex.X,
            _ => throw AltitudeMapGeneratorException.NotProperRiverEndnodeDirection(Direction)
        };
        return value.ApproxOperatorTo(OperatorType, CompareValue);
    }
}
