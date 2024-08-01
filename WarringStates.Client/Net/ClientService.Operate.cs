using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript;
using Microsoft.VisualBasic.Logging;
using LocalUtilities.TypeToolKit.Convert;
using System.Net;
using WarringStates.Client.Events;
using WarringStates.Client.User;
using WarringStates.Flow;
using WarringStates.Net.Common;
using WarringStates.User;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;
using WarringStates.Client.Map;

namespace WarringStates.Client.Net;

partial class ClientService
{
    public event NetEventHandler<string[]>? OnUpdatePlayerList;

    public void Login(string address, int port, string name, string password)
    {
        try
        {
            if (IsLogined)
                return;
            var host = new IPEndPoint(IPAddress.Parse(address), port);
            if (!((ClientProtocol)Protocol).Connect(host))
                throw new NetException(ServiceCode.NoConnection);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Login, (byte)OperateCode.None)
               .AppendArgs(ServiceKey.Name, name)
               .AppendArgs(ServiceKey.Password, password);
            SendCommand(sender);
            LoginDone?.WaitOne(ConstTabel.BlockkMilliseconds);
        }
        catch (Exception ex)
        {
            Dispose();
            this.HandleException(ex);
        }
    }

    private void HandleLogin(CommandReceiver receiver)
    {
        ReceiveCallback(receiver);
        Player = receiver.GetArgs<Player>(ServiceKey.Player);
        IsLogined = true;
        LoginDone.Set();
        HandleLogined();
    }

    public void SendMessage(string message, string receivePlayerId)
    {
        try
        {
            var data = SerializeTool.Serialize(message, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Request, data, 0, data.Length)
                .AppendArgs(ServiceKey.ReceiveName, receivePlayerId)
                .AppendArgs(ServiceKey.SendName, Player.Name);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void HandleMessage(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Request)
        {
            var message = FormatMessage(receiver);
            this.HandleLog(message);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback, receiver.Data, 0, receiver.Data.Length)
                    .AppendArgs(ServiceKey.ReceiveName, receiver.GetArgs<string>(ServiceKey.ReceiveName))
                    .AppendArgs(ServiceKey.SendName, receiver.GetArgs<string>(ServiceKey.SendName));
            CallbackSuccess(sender);
        }
        else if (operateCode is OperateCode.Callback)
        {
            ReceiveCallback(receiver);
            var message = FormatMessage(receiver);
            this.HandleLog(message);
        }
        else if (operateCode is OperateCode.Broadcast)
        {
            var message = FormatMessage(receiver);
            this.HandleLog(message);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode, receiver.Data, 0, receiver.Data.Length)
                    .AppendArgs(ServiceKey.ReceiveName, receiver.GetArgs<string>(ServiceKey.ReceiveName))
                    .AppendArgs(ServiceKey.SendName, receiver.GetArgs<string>(ServiceKey.SendName));
            CallbackSuccess(sender);
        }
    }

    private void HandlePlayer(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.List)
        {
            var playerList = receiver.GetArgs<string[]>(ServiceKey.List);
            OnUpdatePlayerList?.Invoke(playerList);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackSuccess(sender);
        }
    }

    public void FetchArchive(string archiveId)
    {
        var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Archive, (byte)OperateCode.Request)
            .AppendArgs(ServiceKey.Id, archiveId);
        SendCommand(sender);
    }

    public void JoinArchive(string archiveId)
    {
        var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Archive, (byte)OperateCode.Join)
            .AppendArgs(ServiceKey.Id, archiveId);
        SendCommand(sender);
    }

    private void HandleArchive(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.List)
        {
            var infoList = receiver.GetArgs<ArchiveInfo[]>(ServiceKey.List);
            LocalArchives.ReLocate(infoList);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackSuccess(sender);
        }
        else if (operateCode is OperateCode.Request)
        {
            ReceiveCallback(receiver);
            Task.Run(() =>
            {
                var playerArchive = receiver.GetArgs<PlayerArchive>(ServiceKey.Archive);
                LocalArchives.SetCurrentArchive(playerArchive);
            });
        }
        else if (operateCode is OperateCode.Join)
        {
            ReceiveCallback(receiver);
            var playerArchive = receiver.GetArgs<PlayerArchive>(ServiceKey.Archive);
            LocalArchives.StartPlayArchive(playerArchive);
            var sender = new CommandSender(DateTime.Now, receiver.CommandCode, (byte)OperateCode.Callback);
            SendCommand(sender);
        }
        else if (operateCode is OperateCode.Callback)
        {
            ReceiveCallback(receiver);
        }
    }

    private void HandleTimer(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Update)
        {
            var args = receiver.GetArgs<SpanFlowTickOnArgs>(ServiceKey.Args);
            LocalEvents.TryBroadcast(LocalEvents.Flow.SpanFlowTickOn, args);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackSuccess(sender);
        }
    }

    public void CheckBuildLand(Coordinate site)
    {
        var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Land, (byte)OperateCode.Check)
            .AppendArgs(ServiceKey.Site, site);
        SendCommand(sender);
    }

    public void BuildLand(Coordinate site, SourceLandTypes type)
    {
        var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Land, (byte)OperateCode.Update)
           .AppendArgs(ServiceKey.Site, site)
           .AppendArgs(ServiceKey.Type, type);
        SendCommand(sender);
    }

    private void HandleLand(CommandReceiver receiver)
    {

        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Check)
        {
            ReceiveCallback(receiver);
            var args = receiver.GetArgs<SourceLandCanBuildArgs>(ServiceKey.Args);
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.SourceLandCanBuild, args);
        }
        else if (operateCode is OperateCode.Update)
        {
            ReceiveCallback(receiver);
            var vision = receiver.GetArgs<VisibleLands>(ServiceKey.Object);
            Atlas.AddVision(vision);
        }
    }
}
