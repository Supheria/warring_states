﻿using LocalUtilities.General;
using LocalUtilities.IocpNet;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WarringStates.Flow;
using WarringStates.Net.Common;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace WarringStates.Server.GUI.Models;

internal class ServiceManager : INetLogger
{
    public event NetEventHandler<int>? OnConnectionCountChange;

    public event NetEventHandler? OnStart;

    public event NetEventHandler? OnClose;

    public NetEventHandler<string>? OnLog { get; set; }

    Socket? Socket { get; set; } = null;

    public bool IsStart { get; private set; } = false;

    Roster<string, ServerService> Players { get; } = [];

    SpanFlow SpanFlow { get; } = new();

    public ServiceManager()
    {
        SpanFlow.Tick += UpdateCurrentDate;
    }

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
            var localEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            Socket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(localEndPoint);
            Socket.Listen();
            AcceptAsync(null);
            IsStart = true;
            EnableListener();
            this.HandleLog("start");
            OnStart?.Invoke();
            SpanFlow.Relocate(AtlasEx.CurrentSpan);
            SpanFlow.Start();
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
            foreach (var service in Players)
                service.Dispose();
            Socket?.Close();
            IsStart = false;
            DisableListener();
            this.HandleLog("close");
            OnClose?.Invoke();
            // TODO: stop spanflow
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void EnableListener()
    {
        //LocalEvents.TryAddListener<CommandRelayArgs>(LocalEvents.NetService.RelayCommand, RelayCommand);
    }

    private void DisableListener()
    {
        //LocalEvents.TryRemoveListener<CommandRelayArgs>(LocalEvents.NetService.RelayCommand, RelayCommand);
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
        service.OnLogined += () => AddPlayer(service);
        service.OnClosed += () => RemovePlayer(service);
        service.Accept(acceptArgs.AcceptSocket);
    ACCEPT:
        if (acceptArgs.SocketError is SocketError.Success)
            AcceptAsync(acceptArgs);
    }

    private void AddPlayer(ServerService service)
    {
        if (!AtlasEx.AddPlayer(service.Player) ||
            !Players.TryAdd(service))
        {
            service.Dispose();
            return;
        }
        service.OnClosed += () =>
        {
            RemovePlayer(service);
        };
        service.Server = this;
        HandleUpdateConnection();
    }

    private void RemovePlayer(ServerService service)
    {
        if (!(Players.TryGetValue(service.Signature, out var toCheck) && toCheck.TimeStamp == service.TimeStamp))
            return;
        Players.TryRemove(service);
        HandleUpdateConnection();
    }

    public void BroadcastMessage(string message)
    {
        Parallel.ForEach(Players, service =>
        {
            service.SendMessage(message);
        });
    }

    public void RelayCommand(CommandReceiver receiver)
    {
        try
        {
            var name = (OperateCode)receiver.OperateCode switch
            {
                OperateCode.Request => receiver.GetArgs<string>(ServiceKey.ReceiveName),
                OperateCode.Callback => receiver.GetArgs<string>(ServiceKey.SendName),
                _ => null,
            } ?? throw new NetException(ServiceCode.MissingCommandArgs, ServiceKey.ReceiveName, ServiceKey.SendName);
            if (!Players.TryGetValue(name, out var user))
                throw new NetException(ServiceCode.PlayerNotExist);
            user.HandleCommand(receiver);

        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void UpdateCurrentDate(SpanFlowTickOnArgs args)
    {
        Parallel.ForEach(Players, service =>
        {
            if (service.Joined)
                service.UpdateCurrentDate(args);
        });
    }

    public void HandleUpdateConnection()
    {
        OnConnectionCountChange?.Invoke(Players.Count);
        var playerList = Players.Select(x => x.Player.Name).ToArray();
        Parallel.ForEach(Players, service =>
        {
            service.UpdatePlayerList(playerList);
        });
    }
}
