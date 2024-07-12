using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WarringStates.Net.Common;

namespace WarringStates.Server.Net;

internal class Server : INetLogger
{
    public event NetEventHandler<int>? OnConnectionCountChange;

    public NetEventHandler<string>? OnLog { get; set; }

    Socket? Socket { get; set; } = null;

    public bool IsStart { get; private set; } = false;

    ConcurrentDictionary<string, ServerService> UserMap { get; } = [];

    public string GetLog(string message)
    {
        return new StringBuilder()
            .Append(SignTable.OpenParenthesis)
            .Append("host")
            .Append(SignTable.CloseParenthesis)
            .Append(SignTable.Space)
            .Append(message)
            .Append(SignTable.Space)
            .Append(SignTable.At)
            .Append(DateTime.Now.ToString(DateTimeFormat.Outlook))
            .ToString();
    }

    public void Start(int port)
    {
        try
        {
            if (IsStart)
                throw new NetException(ServiceCode.ServerHasStarted);
            // 使用0.0.0.0作为绑定IP，则本机所有的IPv4地址都将绑定
            var localEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            Socket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(localEndPoint);
            Socket.Listen();
            AcceptAsync(null);
            IsStart = true;
            this.HandleLog("start");
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    public void Close()
    {
        try
        {
            if (!IsStart)
                throw new NetException(ServiceCode.ServerNotStartYet);
            foreach (var service in UserMap.Values)
                service.Dispose();
            Socket?.Close();
            IsStart = false;
            this.HandleLog("close");
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void AcceptAsync(SocketAsyncEventArgs? acceptArgs)
    {
        if (acceptArgs == null)
        {
            acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += (_, args) => ProcessAccept(args);
        }
        else
        {
            acceptArgs.AcceptSocket = null; //释放上次绑定的Socket，等待下一个Socket连接
        }
        if (Socket is not null && !Socket.AcceptAsync(acceptArgs))
            ProcessAccept(acceptArgs);
    }

    private void ProcessAccept(SocketAsyncEventArgs acceptArgs)
    {
        if (acceptArgs.AcceptSocket is null)
            goto ACCEPT;
        var service = new ServerService();
        service.OnLog += this.HandleLog;
        service.OnLogined += () => AddService(service);
        service.OnClosed += () => RemoveService(service);
        service.OnCommand += HandleOperate;
        service.Accept(acceptArgs.AcceptSocket);
    ACCEPT:
        if (acceptArgs.SocketError is SocketError.Success)
            AcceptAsync(acceptArgs);
    }

    private void AddService(ServerService service)
    {
        if (service.UserInfo is null || service.UserInfo.Name is "" ||
            !UserMap.TryAdd(service.UserInfo.Name, service))
        {
            service.Dispose();
            return;
        }
        HandleUpdateConnection();
    }

    private void RemoveService(ServerService service)
    {
        if (service.UserInfo is null || service.UserInfo.Name is "" ||
            !(UserMap.TryGetValue(service.UserInfo.Name, out var toCheck) && toCheck.TimeStamp == service.TimeStamp))
            return;
        UserMap.TryRemove(service.UserInfo.Name, out _);
        HandleUpdateConnection();
    }

    private void HandleOperate(CommandReceiver receiver)
    {
        try
        {
            var userName = receiver.GetArgs(ServiceKey.ReceiveUser);
            if (!UserMap.TryGetValue(userName, out var user))
                throw new NetException(ServiceCode.UserNotExist);
            user.DoCommand(receiver);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void HandleException(Exception ex)
    {
        // TODO:
        this.HandleLog(ex.Message);
    }

    public void BroadcastMessage(string message)
    {
        foreach (var service in UserMap.Values)
            service.SendMessage(message);
    }

    public void BroadcastUserList()
    {
        var users = UserMap.Keys.ToArray();
        foreach (var service in UserMap.Values)
            service.UpdateUserList(users);
    }

    public void HandleUpdateConnection()
    {
        OnConnectionCountChange?.Invoke(UserMap.Count);
        BroadcastUserList();
    }
}
