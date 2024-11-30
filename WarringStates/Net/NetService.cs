using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities;
using System.Text;
using WarringStates.Net.Common;
using WarringStates.Net.Utilities;
using WarringStates.User;

namespace WarringStates.Net;

public abstract class NetService : INetLogger
{
    public const int CommandLengthMax = 1024 * 1024 * 2;

    public const int DataLengthMax = 1024 * 1024;

    public event NetEventHandler? OnLogined;

    public event NetEventHandler? OnClosed;

    public event NetEventHandler<string>? OnProcessing;

    protected delegate void CommandHandler(CommandReceiver receiver);

    public NetEventHandler<string>? OnLog { get; set; }

    AutoDisposeItemCollection<CommandWaitingCallback> CommandsWaitingCallback { get; } = [];

    AutoDisposeItemCollection<CommandWaitingCompose> CommandsWaitingCompose { get; } = [];

    public bool IsLogined { get; protected set; } = false;

    public Player Player { get; protected set; } = new();

    protected AutoDisposeItemCollection<AutoDisposeFileStream> AutoFiles { get; } = [];

    protected Protocol Protocol { get; }

    protected Dictionary<CommandCode, CommandHandler> HandleCommands { get; } = [];

    protected static SsSignTable SignTable { get; } = new();

    public NetService(Protocol protocol)
    {
        Protocol = protocol;
        Protocol.OnLog += this.HandleLog;
        Protocol.OnDisposed += () =>
        {
            foreach (var autoFile in AutoFiles)
                autoFile.Dispose();
            IsLogined = false;
            this.HandleLog("close");
            OnClosed?.Invoke();
        };
        Protocol.OnReceiveCommand += ReceiveAsync;
    }

    private void ReceiveAsync(CommandReceiver receiver)
    {
        try
        {
            if ((CommandCode)receiver.CommandCode is not CommandCode.ComposeCommand)
            {
                HandleCommand(receiver);
                return;
            }
            var operateCode = (OperateCode)receiver.OperateCode;
            if (operateCode is OperateCode.Start)
            {
                var waitingCompose = new CommandWaitingCompose(receiver);
                waitingCompose.OnLog += this.HandleLog;
                if (!CommandsWaitingCompose.TryAdd(waitingCompose))
                    throw new NetException(ServiceCode.CannotAddCommandWaitingForCompose);
                waitingCompose.StartWaiting();
            }
            else if (operateCode is OperateCode.Continue)
            {
                if (!CommandsWaitingCompose.TryGetValue(receiver.TimeStamp, out var waitingCompose))
                    throw new NetException(ServiceCode.CannotFindSourceCommandToCompose);
                waitingCompose.AppendCommand(receiver);
            }
            else if (operateCode is OperateCode.Finish)
            {
                if (!CommandsWaitingCompose.TryGetValue(receiver.TimeStamp, out var waitingCompose))
                    throw new NetException(ServiceCode.CannotFindSourceCommandToCompose);
                var commandReceive = waitingCompose.GetCommand();
                waitingCompose.Dispose();
                HandleCommand(commandReceive);
            }
            else
                throw new NetException(ServiceCode.UnknownOperate);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void SendAsync(CommandSender sender)
    {
        var packet = sender.GetPacket();
        if (packet.Length <= CommandLengthMax)
        {
            Protocol.SendAsync(sender);
            return;
        }
        var timeStamp = sender.TimeStamp;
        var commandInfo = new byte[2] { sender.CommandCode, sender.OperateCode };
        sender = new CommandSender(timeStamp, (byte)CommandCode.ComposeCommand, (byte)OperateCode.Start, commandInfo, 0, 2);
        Protocol.SendAsync(sender);
        var offset = 0;
        while (offset < packet.Length)
        {
            var count = Math.Min(packet.Length - offset, CommandLengthMax);
            sender = new CommandSender(timeStamp, (byte)CommandCode.ComposeCommand, (byte)OperateCode.Continue, packet, offset, count);
            Protocol.SendAsync(sender);
            offset += count;
        }
        sender = new CommandSender(timeStamp, (byte)CommandCode.ComposeCommand, (byte)OperateCode.Finish);
        Protocol.SendAsync(sender);
    }

    public abstract void HandleCommand(CommandReceiver receiver);

    public abstract string GetLog(string message);

    public void Dispose()
    {
        Protocol.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="doRetry"></param>
    /// <exception cref="NetException"></exception>
    public void SendCommand(CommandSender sender)
    {
        var waitingCallback = new CommandWaitingCallback(sender);
        waitingCallback.OnLog += this.HandleLog;
        if (!CommandsWaitingCallback.TryAdd(waitingCallback))
            throw new NetException(ServiceCode.CannotAddCommandWaitingForCallback);
        waitingCallback.StartWaiting();
        SendAsync(sender);
    }

    public void CallbackSuccess(CommandSender sender)
    {
        sender.AppendArgs(ServiceKey.CallbackCode, ServiceCode.Success);
        SendAsync(sender);
    }

    public void CallbackFailure(CommandSender sender, Exception ex)
    {
        var errorCode = ex switch
        {
            NetException iocp => iocp.ErrorCode,
            _ => ServiceCode.UnknowError,
        };
        sender.AppendArgs(ServiceKey.CallbackCode, errorCode);
        sender.AppendArgs(ServiceKey.ErrorMessage, ex.Message);
        SendAsync(sender);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns></returns>
    /// <exception cref="NetException"></exception>
    public void ReceiveCallback(CommandReceiver receiver)
    {
        if (!CommandsWaitingCallback.TryGetValue(receiver.TimeStamp, out var commandSend))
            throw new NetException(ServiceCode.CannotFindSourceSendCommand);
        commandSend.Dispose();
        var callbackCode = receiver.GetArgs<ServiceCode>(ServiceKey.CallbackCode);
        if (callbackCode is ServiceCode.Success)
            return;
        var errorMessage = receiver.GetArgs<string>(ServiceKey.ErrorMessage);
        // TODO: classify and handle different exception types
        throw new NetException(callbackCode, errorMessage);
    }

    protected void HandleLogined()
    {
        this.HandleLog("login");
        OnLogined?.Invoke();
    }

    protected void HandleUploadStart()
    {
        this.HandleLog("upload file start...");
    }

    protected void HandleDownloadStart()
    {
        this.HandleLog("download file start...");
    }

    protected void HandleUploading(long fileLength, long position)
    {
        var message = new StringBuilder()
            .Append("uploading")
            .Append(Math.Round(position * 100d / fileLength, 2))
            .Append(SignCollection.Percent)
            .ToString();
        OnProcessing?.Invoke(message);
    }

    protected void HandleDownloading(long fileLength, long position)
    {
        var message = new StringBuilder()
            .Append("downloading")
            .Append(Math.Round(position * 100d / fileLength, 2))
            .Append(SignCollection.Percent)
            .ToString();
        OnProcessing?.Invoke(message);
    }

    protected void HandleUploaded(DateTime startTime)
    {
        var span = DateTime.Now - startTime;
        var message = new StringBuilder()
            .Append("upload file success")
            .Append(SignCollection.OpenParenthesis)
            .Append(Math.Round(span.TotalMilliseconds, 2))
            .Append("ms")
            .Append(SignCollection.CloseParenthesis)
            .ToString();
        this.HandleLog(message);
        OnProcessing?.Invoke(message);
    }

    protected void HandleDownloaded(DateTime startTime)
    {
        var span = DateTime.Now - startTime;
        var message = new StringBuilder()
            .Append("download file success")
            .Append(SignCollection.OpenParenthesis)
            .Append(Math.Round(span.TotalMilliseconds, 2))
            .Append("ms")
            .Append(SignCollection.CloseParenthesis)
            .ToString();
        this.HandleLog(message);
        OnProcessing?.Invoke(message);
    }

    protected static string FormatMessage(CommandReceiver receiver)
    {
        return new StringBuilder()
            .Append(receiver.GetArgs<string>(ServiceKey.SendName))
            .Append(SignCollection.Sub)
            .Append(SignCollection.Greater)
            .Append(receiver.GetArgs<string>(ServiceKey.ReceiveName))
            .Append(SignCollection.Colon)
            .Append(SignCollection.Space)
            .Append(SerializeTool.Deserialize<string>(new(), receiver.Data, 0, receiver.Data.Length, SignTable, null))
            .ToString();
    }
}
