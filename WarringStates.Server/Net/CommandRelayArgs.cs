using LocalUtilities.IocpNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;

namespace WarringStates.Server.Net;

internal class CommandRelayArgs(CommandReceiver receiver) : ICallbackArgs
{
    public CommandReceiver Receiver { get; set; } = receiver;
}
