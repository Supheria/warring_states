using WarringStates.Events;

namespace WarringStates.Client.Graph;

internal class PointOnGridCellArgs(Point realPoint) : ICallbackArgs
{
    public Point RealPoint { get; } = realPoint;
}
