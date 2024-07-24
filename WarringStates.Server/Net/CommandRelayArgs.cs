using LocalUtilities.IocpNet.Common;
using WarringStates.Events;

namespace WarringStates.Server.Net;

internal class CommandRelayArgs(CommandReceiver receiver) : ICallbackArgs
{
    public CommandReceiver Receiver { get; set; } = receiver;
}
