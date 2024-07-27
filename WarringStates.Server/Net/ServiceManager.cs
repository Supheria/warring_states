using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WarringStates.Net.Common;
using WarringStates.Server.Events;
using WarringStates.Server.User;

namespace WarringStates.Server.Net;

internal class ServiceManager : INetLogger
{
    public event NetEventHandler<int>? OnConnectionCountChange;

    public event NetEventHandler? OnStart;

    public event NetEventHandler? OnClose;

    public NetEventHandler<string>? OnLog { get; set; }

    Socket? Socket { get; set; } = null;

    public bool IsStart { get; private set; } = false;

    ServiceRoster PlayerMap { get; } = [];

    ConcurrentDictionary<string, CacheArchive> CacheArchives { get; } = [];

    PlayerGroupRoster PlayerGroups { get; } = [];

    public string GetLog(string message)
    {
        return new StringBuilder()
            .Append(SignCollection.OpenParenthesis)
            .Append(nameof(ServiceManager))
            .Append(SignCollection.CloseParenthesis)
            .Append(SignCollection.Space)
            .Append(message)
            .Append(SignCollection.Space)
            .Append(SignCollection.At)
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
            EnableListener();
            this.HandleLog("start");
            OnStart?.Invoke();
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void Close()
    {
        try
        {
            if (!IsStart)
                throw new NetException(ServiceCode.ServerNotStartYet);
            foreach (var service in PlayerMap)
                service.Dispose();
            Socket?.Close();
            IsStart = false;
            DisableListener();
            this.HandleLog("close");
            OnClose?.Invoke();
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void EnableListener()
    {
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, BroadcastArchiveList);
        //LocalEvents.TryAddListener<CommandRelayArgs>(LocalEvents.NetService.RelayCommand, RelayCommand);
    }

    private void DisableListener()
    {
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.ArchiveListRefreshed, BroadcastArchiveList);
        //LocalEvents.TryRemoveListener<CommandRelayArgs>(LocalEvents.NetService.RelayCommand, RelayCommand);
    }

    private void BroadcastArchiveList()
    {
        Parallel.ForEach(PlayerMap, service =>
        {
            service.UpdateArchiveList();
        });
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
        service.OnRequestArchive += (receiver) => RequestArchive(receiver, service);
        service.OnJoinArchive += (receiver) => JoinArchive(receiver, service);
        service.Accept(acceptArgs.AcceptSocket);
    ACCEPT:
        if (acceptArgs.SocketError is SocketError.Success)
            AcceptAsync(acceptArgs);
    }

    private void RequestArchive(CommandReceiver receiver, ServerService service)
    {
        try
        {
            var archiveId = receiver.GetArgs<string>(ServiceKey.Id) ??
                throw new NetException(ServiceCode.MissingCommandArgs, ServiceKey.Id);
            if (!LocalArchive.Archives.TryGetValue(archiveId, out var info))
                throw new NetException(ServiceCode.NoMatchArchiveId);
            if (CacheArchives.TryGetValue(archiveId, out var cache))
            {
                service.ResbonseArchiveRequestOrJoin(receiver, LocalArchive.GetPlayerArchive(info, cache.LandMap, service.Player.Id));
                return;
            }
            if (PlayerGroups.TryGetValue(archiveId, out var group))
            {
                service.ResbonseArchiveRequestOrJoin(receiver, LocalArchive.GetPlayerArchive(info, group.LandMap, service.Player.Id));
                return;
            }
            cache = new(info);
            cache.OnDisposed += () => CacheArchives.TryRemove(archiveId, out _);
            CacheArchives.TryAdd(archiveId, cache);
            service.ResbonseArchiveRequestOrJoin(receiver, LocalArchive.GetPlayerArchive(info, cache.LandMap, service.Player.Id));
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void AddService(ServerService service)
    {
        if (!PlayerMap.TryAdd(service))
        {
            service.Dispose();
            return;
        }
        HandleUpdateConnection();
    }

    private void RemoveService(ServerService service)
    {
        if (!(PlayerMap.TryGetValue(service.Signature, out var toCheck) && toCheck.TimeStamp == service.TimeStamp))
            return;
        PlayerMap.TryRemove(service);
        HandleUpdateConnection();
    }

    private void JoinArchive(CommandReceiver receiver, ServerService service)
    {
        try
        {
            var archiveId = receiver.GetArgs<string>(ServiceKey.Id) ??
                throw new NetException(ServiceCode.MissingCommandArgs, ServiceKey.Id);
            if (!LocalArchive.Archives.TryGetValue(archiveId, out var info))
                return;
            if (PlayerGroups.TryGetValue(archiveId, out var group))
            {
                group.AddPlayer(service);
                service.ResbonseArchiveRequestOrJoin(receiver, LocalArchive.GetPlayerArchive(info, group.LandMap, service.Player.Id));
                return;
            }
            if (CacheArchives.TryGetValue(archiveId, out var cache))
                cache.Dispose();
            group = new(info);
            group.AddPlayer(service);
            if (!PlayerGroups.TryAdd(group))
                group.RemovePlayer(service);
            else
                service.ResbonseArchiveRequestOrJoin(receiver, LocalArchive.GetPlayerArchive(info, group.LandMap, service.Player.Id));
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void LeaveArchive(string archiveId, ServerService service)
    {
        if (!PlayerGroups.TryGetValue(archiveId, out var group))
            return;
        group.RemovePlayer(service);
        if (group.PlayerCount < 1)
            PlayerGroups.TryRemove(group);
    }

    //private void RelayCommand(CommandRelayArgs args)
    //{
    //    try
    //    {
    //        var userId = (OperateCode)args.Receiver.OperateCode switch
    //        {
    //            OperateCode.Request => args.Receiver.GetArgs<string>(ServiceKey.ReceivePlayer),
    //            OperateCode.Callback => args.Receiver.GetArgs<string>(ServiceKey.SendPlayer),
    //            _ => null,
    //        } ?? throw new NetException(ServiceCode.MissingCommandArgs, ServiceKey.ReceivePlayer, ServiceKey.SendPlayer);
    //        if (!PlayerMap.TryGetValue(userId, out var user))
    //            throw new NetException(ServiceCode.PlayerNotExist);
    //        user.HandleCommand(args.Receiver);
    //    }
    //    catch (Exception ex)
    //    {
    //        this.HandleException(ex);
    //    }
    //}

    public void BroadcastMessage(string message)
    {
        Parallel.ForEach(PlayerMap, service =>
        {
            service.SendMessage(message);
        });
    }

    public void UpdatePlayerGroupList()
    {
        var players = new Dictionary<string, string>();
        //foreach
        //foreach (var service in PlayerMap.Values)
        //    service.UpdatePlayerList(players);
    }

    public void HandleUpdateConnection()
    {
        OnConnectionCountChange?.Invoke(PlayerMap.Count);
        //BroadcastUserList();
    }
}
