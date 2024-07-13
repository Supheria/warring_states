using LocalUtilities.IocpNet.Common;
using WarringStates.Net.Common;

namespace WarringStates.Net.Utilities;

internal class CommandWaitingCallback : CommandWaitingHelper
{
    public CommandWaitingCallback(CommandSender sender)
    {
        CommandCode = (CommandCode)sender.CommandCode;
        OperateCode = (OperateCode)sender.OperateCode;
        TimeStamp = sender.TimeStamp;
    }
}
