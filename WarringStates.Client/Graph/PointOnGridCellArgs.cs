using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;

namespace WarringStates.Client.Graph;

internal class PointOnGridCellArgs(Point realPoint) : ICallbackArgs
{
    public Point RealPoint { get; } = realPoint;
}
