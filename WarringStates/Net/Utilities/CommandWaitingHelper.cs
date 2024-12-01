using LocalUtilities.General;
using LocalUtilities.IocpNet;
using System.Text;
using WarringStates.Common;
using WarringStates.Net.Common;

namespace WarringStates.Net.Utilities;

internal abstract class CommandWaitingHelper() : AutoDisposeItem(ConstTabel.CommandWaitingIntervalMilliseconds), INetLogger
{
    public NetEventHandler? OnWaitingFailed { get; set; }

    public NetEventHandler<string>? OnLog { get; set; }

    protected CommandCode CommandCode { get; init; }

    protected OperateCode OperateCode { get; init; }

    public string GetLog(string message)
    {
        return message;
    }

    public void StartWaiting()
    {
        DaemonThread.Start();
    }

    protected override void AutoDispose()
    {
        Dispose();
        OnWaitingFailed?.Invoke();
        HandleWaitingFailed();
        return;
    }

    private void HandleWaitingFailed()
    {
        var message = new StringBuilder()
            .Append(SignCollection.OpenBracket)
            .Append(StringTable.WaitingCallback)
            .Append(SignCollection.Space)
            .Append(StringTable.Failed)
            .Append(SignCollection.CloseBracket)
            .Append(SignCollection.Space)
            .Append(CommandCode)
            .Append(SignCollection.Comma)
            .Append(SignCollection.Space)
            .Append(OperateCode)
            .ToString();
        this.HandleLog(message);
    }
}
