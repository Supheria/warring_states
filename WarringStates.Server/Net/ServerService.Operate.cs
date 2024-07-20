using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using WarringStates.Net.Common;
using WarringStates.Server.User;
using WarringStates.User;
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeToolKit.Convert;

namespace WarringStates.Server.Net;

partial class ServerService
{
    public NetEventHandler<CommandReceiver>? OnRelayCommand;

    private void HandleHeartBeats(CommandReceiver receiver)
    {
        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
        CallbackSuccess(sender);
    }

    private void HandleLogin(CommandReceiver receiver)
    {
        if (IsLogined)
            throw new NetException(ServiceCode.UserAlreadyLogined);
        var name = receiver.GetArgs<string>(ServiceKey.UserName) ?? "";
        var password = receiver.GetArgs<string>(ServiceKey.Password) ?? "";
        // TODO: validate userinfo
        Player = new(name, password);
        IsLogined = true;
        HandleLogined();
        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
        CallbackSuccess(sender);
        UpdateArchiveList();
    }

    public void SendMessage(string message)
    {
        try
        {
            var data = SerializeTool.Serialize(message, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Broadcast, data, 0, data.Length)
                .AppendArgs(ServiceKey.ReceivePlayer, Player.Name)
                .AppendArgs(ServiceKey.SendPlayer, nameof(Server));
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
            if (Player?.Name == receiver.GetArgs<string>(ServiceKey.ReceivePlayer))
            {
                var message = FormatMessage(receiver);
                this.HandleLog(message);
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Request, receiver.Data, 0, receiver.Data.Length)
                    .AppendArgs(ServiceKey.ReceivePlayer, receiver.GetArgs<string>(ServiceKey.ReceivePlayer))
                    .AppendArgs(ServiceKey.SendPlayer, receiver.GetArgs<string>(ServiceKey.SendPlayer));
                SendCommand(sender);
            }
            else
                OnRelayCommand?.Invoke(receiver);
        }
        else if (operateCode is OperateCode.Callback)
        {
            if (Player?.Name == receiver.GetArgs<string>(ServiceKey.SendPlayer))
            {
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback, receiver.Data, 0, receiver.Data.Length)
                    .AppendArgs(ServiceKey.ReceivePlayer, receiver.GetArgs<string>(ServiceKey.ReceivePlayer))
                    .AppendArgs(ServiceKey.SendPlayer, receiver.GetArgs<string>(ServiceKey.SendPlayer));
                CallbackSuccess(sender);
            }
            else
            {
                ReceiveCallback(receiver);
                OnRelayCommand?.Invoke(receiver);
            }
        }
        else if (operateCode is OperateCode.Broadcast)
        {
            ReceiveCallback(receiver);
            var message = FormatMessage(receiver);
            this.HandleLog(message);
        }
    }

    public void UpdatePlayerList(string[] nameList)
    {
        try
        {
            var data = SerializeTool.Serialize(nameList, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Player, (byte)OperateCode.List, data, 0, data.Length);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void UpdateArchiveList()
    {
        try
        {
            //var playerArchiveInfoList = new List<PlayerArchive>();
            //var index = 0;
            //while (LocalArchives.TryGetArchiveInfo(index, out var info))
            //{
            //    var players = info.LoadPlayers();
            //    var sourceLands = info.LoadSourceLands();
            //    if (!sourceLands.TryGetValue(Player.Id, out var ownLands))
            //        ownLands = [];
            //    var playerArchiveInfo = new PlayerArchive(info.Id, info.WorldName, info.WorldSize, players.Count, ownLands);
            //    playerArchiveInfoList.Add(playerArchiveInfo);
            //    index++;
            //}
            //var data = SerializeTool.Serialize(playerArchiveInfoList, new(ServiceKey.ArchiveList), SignTable, null);
            //var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Archive, (byte)OperateCode.List, data, 0, data.Length);
            //SendCommand(sender);
            var data = SerializeTool.Serialize(LocalArchives.ArchiveInfoList, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Archive, (byte)OperateCode.List, data, 0, data.Length);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void HandleArchive(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode == OperateCode.List)
        {
            ReceiveCallback(receiver);
        }
        else if (operateCode is OperateCode.Join)
        {
            // TODO: join archive
        }
    }
}
