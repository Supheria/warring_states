﻿using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using System.Text;
using WarringStates.Common;
using WarringStates.Net.Common;

namespace WarringStates.Net.Utilities;

internal abstract class CommandWaitingHelper() : AutoDisposeItem(ConstTabel.CommandWaitingIntervalMilliseconds), INetLogger
{
    public NetEventHandler? OnWaitingFailed;

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