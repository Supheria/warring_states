using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using System.Text;
using WarringStates.Common;

namespace WarringStates.Net.Common;

internal class CommandWaitingCallback : CommandWaitingHelper
{
    public CommandWaitingCallback(CommandSender sender)
    {
        CommandCode = (CommandCode)sender.CommandCode;
        OperateCode = (OperateCode)sender.OperateCode;
        TimeStamp = sender.TimeStamp;
    }
}
