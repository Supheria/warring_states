using LocalUtilities.TypeToolKit.EventProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

internal class GameFormUpdateEventArgument(Size clientSize) : IEventArgument
{
    public Size ClientSize { get; } = clientSize;
}
