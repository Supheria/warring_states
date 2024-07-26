using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript;
using System.Net;
using WarringStates.Client.Events;
using WarringStates.Client.User;
using WarringStates.Map.Terrain;
using WarringStates.Net.Common;
using WarringStates.User;

namespace WarringStates.Client.Net;

partial class ClientService
{
    public event NetEventHandler<string[]>? OnUpdatePlayerList;

    private void HeartBeats()
    {
        try
        {
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.HeartBeats, (byte)OperateCode.None);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

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
               .AppendArgs(ServiceKey.UserName, name)
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
        Player = SerializeTool.Deserialize<Player>(new(), receiver.Data, 0, receiver.Data.Length, SignTable, null) ??
            throw new NetException(ServiceCode.MissingCommandArgs, nameof(Player));
        IsLogined = true;
        LoginDone.Set();
        HandleLogined();
        DaemonThread.Start();
    }

    public void SendMessage(string message, string receivePlayerId)
    {
        try
        {
            var data = SerializeTool.Serialize(message, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Request, data, 0, data.Length)
                .AppendArgs(ServiceKey.ReceivePlayer, receivePlayerId)
                .AppendArgs(ServiceKey.SendPlayer, Player.Id);
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
                    .AppendArgs(ServiceKey.ReceivePlayer, receiver.GetArgs<string>(ServiceKey.ReceivePlayer))
                    .AppendArgs(ServiceKey.SendPlayer, receiver.GetArgs<string>(ServiceKey.SendPlayer));
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
                    .AppendArgs(ServiceKey.ReceivePlayer, receiver.GetArgs<string>(ServiceKey.ReceivePlayer))
                    .AppendArgs(ServiceKey.SendPlayer, receiver.GetArgs<string>(ServiceKey.SendPlayer));
            CallbackSuccess(sender);
        }
    }

    private void HandlePlayer(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.List)
        {
            var nameList = SerializeTool.Deserialize<string[]>(new(), receiver.Data, 0, receiver.Data.Length, SignTable, null) ?? [];
            OnUpdatePlayerList?.Invoke(nameList);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackSuccess(sender);
        }
    }

    public void FetchThumbnail(string archiveId)
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
            var infoList = SerializeTool.Deserialize<ArchiveInfo[]>(new(), receiver.Data, 0, receiver.Data.Length, SignTable, null);
            LocalArchives.ReLocate(infoList ?? []);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.CommandCode);
            CallbackSuccess(sender);
        }
        else if (operateCode is OperateCode.Request)
        {
            ReceiveCallback(receiver);
            var playerArchive = SerializeTool.Deserialize<PlayerArchive>(new(), receiver.Data, 0, receiver.Data.Length, SignTable, null) ?? new();
            var args = new FetchPlayerArchiveArgs(playerArchive);
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.PlayerAchiveFetched, args);
        }
    }
}
