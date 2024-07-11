using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using System.Text;
using WarringStates.Common;

namespace WarringStates.Net.Common;

public class CommandWaitingCallback : INetLogger
{
    public NetEventHandler? OnWaitingCallbackFailed;

    public event NetEventHandler? OnWasted;
    public NetEventHandler<string>? OnLog { get; set; }

    DaemonThread DaemonThread { get; }

    CommandCode CommandCode { get; }

    OperateCode OperateCode { get; }

    public CommandWaitingCallback(CommandSender sender)
    {
        DaemonThread = new(ConstTabel.WaitingCallbackMilliseconds, WaitingFailed);
        CommandCode = (CommandCode)sender.CommandCode;
        OperateCode = (OperateCode)sender.OperateCode;
    }

    public string GetLog(string message)
    {
        return message;
    }

    public void StartWaiting()
    {
        DaemonThread.Start();
    }

    private void WaitingFailed()
    {
        Waste();
        OnWaitingCallbackFailed?.Invoke();
        HandleWaitingCallbackFailed();
        return;
    }

    public void Waste()
    {
        DaemonThread.Dispose();
        OnWasted?.Invoke();
    }

    private void HandleWaitingCallbackFailed()
    {
        var message = new StringBuilder()
            .Append(SignTable.OpenBracket)
            .Append(StringTable.WaitingCallback)
            .Append(SignTable.Space)
            .Append(StringTable.Failed)
            .Append(SignTable.CloseBracket)
            .Append(SignTable.Space)
            .Append(CommandCode)
            .Append(SignTable.Comma)
            .Append(SignTable.Space)
            .Append(OperateCode)
            .ToString();
        this.HandleLog(message);
    }
}
