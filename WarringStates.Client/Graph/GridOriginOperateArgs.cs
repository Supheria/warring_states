using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;

namespace WarringStates.Client.Graph;

internal class GridOriginOperateArgs : ICallbackArgs
{
    public enum OperateTypes
    {
        Set,
        Offset,
    }

    public OperateTypes Operate { get; }

    public Coordinate Value { get; }

    public GridOriginOperateArgs(OperateTypes operate, Coordinate value)
    {
        Operate = operate;
        Value = value;
    }
}
