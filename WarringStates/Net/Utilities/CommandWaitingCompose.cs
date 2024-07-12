using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Common;
using WarringStates.Net.Common;

namespace WarringStates.Net.Utilities;

internal class CommandWaitingCompose : CommandWaitingHelper
{
    MemoryStream Buffer { get; } = new();

    public CommandWaitingCompose(CommandReceiver receiver)
    {
        CommandCode = (CommandCode)receiver.Data[0];
        OperateCode = (OperateCode)receiver.Data[1];
        TimeStamp = receiver.TimeStamp;
        OnDisposed += Buffer.Dispose;
    }

    public void AppendCommand(CommandReceiver receiver)
    {
        DaemonThread.Stop();
        Buffer.Write(receiver.Data);
        DaemonThread.Start();
    }

    public CommandReceiver GetCommand()
    {
        return new CommandReceiver(Buffer.GetBuffer(), out _);
    }
}
