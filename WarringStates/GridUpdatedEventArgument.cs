using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

internal class GridUpdatedEventArgument(Rectangle drawRect, Coordinate origin) : IEventArgument
{
    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
