using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using System.Collections.Concurrent;
using System.Text;
using WarringStates.Net.Common;

namespace WarringStates.Net;

public abstract class Service : INetLogger
{
    public event NetEventHandler? OnLogined;

    public event NetEventHandler? OnClosed;

    public event NetEventHandler<string>? OnProcessing;

    public event NetEventHandler<CommandReceiver>? OnOperate;

    public event NetEventHandler<CommandReceiver>? OnOperateCallback;

    public NetEventHandler<string>? OnLog { get; set; }

    ConcurrentDictionary<DateTime, CommandWaitingCallback> CommandWaitList { get; } = [];

    protected bool IsLogined { get; set; } = false;

    public UserInfo? UserInfo { get; protected set; } = new();

    protected abstract string RepoPath { get; set; }

    protected AutoDisposeFileStream AutoFile { get; } = new();

    protected DaemonThread? DaemonThread { get; set; }

    protected Protocol Protocol { get; }

    public Service(Protocol protocol)
    {
        Protocol = protocol;
        Protocol.OnLog += this.HandleLog;
        Protocol.OnDisposed += () =>
        {
            AutoFile.Dispose();
            DaemonThread?.Stop();
            IsLogined = false;
            this.HandleLog("close");
            OnClosed?.Invoke();
        };
        Protocol.OnReceiveCommand += Protocol_OnReceiveCommand
            ;
    }

    public abstract string GetLog(string message);

    private void Protocol_OnReceiveCommand(CommandReceiver receiver)
    {
        switch ((CommandCode)receiver.CommandCode)
        {
            case CommandCode.Operate:
                DoOperate(receiver);
                break;
            case CommandCode.OperateCallback:
                DoOperateCallback(receiver);
                break;
        }
    }

    public void Dispose()
    {
        Protocol.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns></returns>
    /// <exception cref="NetException"></exception>
    public void ReceiveCallback(CommandReceiver receiver)
    {
        if (!CommandWaitList.TryGetValue(receiver.TimeStamp, out var commandSend))
            throw new NetException(ServiceCode.CannotFindSourceSendCommand);
        commandSend.Waste();
        var callbackCode = receiver.GetArgs(ServiceKey.CallbackCode).ToEnum<ServiceCode>();
        if (callbackCode is ServiceCode.Success)
            return;
        var errorMessage = receiver.GetArgs(ServiceKey.ErrorMessage);
        throw new NetException(callbackCode, errorMessage);
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
        waitingCallback.OnWasted += () => CommandWaitList.TryRemove(sender.TimeStamp, out _);
        if (!CommandWaitList.TryAdd(sender.TimeStamp, waitingCallback))
            throw new NetException(ServiceCode.CannotWaitSendCommandForCallback);
        waitingCallback.StartWaiting();
        Protocol.SendCommand(sender);
    }

    public void CallbackSuccess(CommandSender sender)
    {
        sender.AppendArgs(ServiceKey.CallbackCode, ServiceCode.Success.ToString());
        Protocol.SendCommand(sender);
    }

    public void CallbackFailure(CommandSender sender, Exception ex)
    {
        var errorCode = ex switch
        {
            NetException iocp => iocp.ErrorCode,
            _ => ServiceCode.UnknowError,
        };
        sender.AppendArgs(ServiceKey.CallbackCode, errorCode.ToString());
        sender.AppendArgs(ServiceKey.ErrorMessage, ex.Message);
        Protocol.SendCommand(sender);
    }

    private void DoOperate(CommandReceiver receiver)
    {
        OnOperate?.Invoke(receiver);
    }

    private void DoOperateCallback(CommandReceiver receiver)
    {
        try
        {
            ReceiveCallback(receiver);
            OnOperateCallback?.Invoke(receiver);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public string GetFileRepoPath(string dirName, string fileName)
    {
        var dir = Path.Combine(RepoPath, dirName);
        if (!Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }
        return Path.Combine(dir, fileName);
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
            .Append(SignTable.Percent)
            .ToString();
        OnProcessing?.Invoke(message);
    }

    protected void HandleDownloading(long fileLength, long position)
    {
        var message = new StringBuilder()
            .Append("downloading")
            .Append(Math.Round(position * 100d / fileLength, 2))
            .Append(SignTable.Percent)
            .ToString();
        OnProcessing?.Invoke(message);
    }

    protected void HandleUploaded(DateTime startTime)
    {
        var span = DateTime.Now - startTime;
        var message = new StringBuilder()
            .Append("upload file success")
            .Append(SignTable.OpenParenthesis)
            .Append(Math.Round(span.TotalMilliseconds, 2))
            .Append("ms")
            .Append(SignTable.CloseParenthesis)
            .ToString();
        this.HandleLog(message);
        OnProcessing?.Invoke(message);
    }

    protected void HandleDownloaded(DateTime startTime)
    {
        var span = DateTime.Now - startTime;
        var message = new StringBuilder()
            .Append("download file success")
            .Append(SignTable.OpenParenthesis)
            .Append(Math.Round(span.TotalMilliseconds, 2))
            .Append("ms")
            .Append(SignTable.CloseParenthesis)
            .ToString();
        this.HandleLog(message);
        OnProcessing?.Invoke(message);
    }
}
