using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;
using WarringStates.Data;
using WarringStates.Flow;
using WarringStates.Map;
using WarringStates.Net.Common;
using WarringStates.Server.Map;
using WarringStates.User;

namespace WarringStates.Server.Net;

partial class ServerService
{
    public event NetEventHandler<CommandReceiver>? OnRequestArchive;

    public event NetEventHandler<CommandReceiver>? OnJoinArchive;

    public ServiceManager Server { get; set; } = new();

    public bool Joined { get; private set; } = false;

    private void HandleLogin(CommandReceiver receiver)
    {
        try
        {
            if (IsLogined)
                throw new NetException(ServiceCode.PlayerAlreadyLogined);
            var name = receiver.GetArgs<string>(ServiceKey.Name);
            var password = receiver.GetArgs<string>(ServiceKey.Password);
            if (!LocalNet.CheckLogin(name, password, out var player, out var code))
                throw new NetException(code);
            Player = player;
            IsLogined = true;
            HandleLogined();
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                .AppendArgs(ServiceKey.Player, Player);
            CallbackSuccess(sender);
            UpdateArchiveList();
        }
        catch (Exception ex)
        {
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackFailure(sender, ex);
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            var data = SerializeTool.Serialize(message, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Broadcast, data, 0, data.Length)
                .AppendArgs(ServiceKey.ReceiveName, Player.Name)
                .AppendArgs(ServiceKey.SendName, nameof(ServiceManager));
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
            if (Player?.Name == receiver.GetArgs<string>(ServiceKey.ReceiveName))
            {
                var message = FormatMessage(receiver);
                this.HandleLog(message);
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Request, receiver.Data, 0, receiver.Data.Length)
                    .AppendArgs(ServiceKey.ReceiveName, receiver.GetArgs<string>(ServiceKey.ReceiveName))
                    .AppendArgs(ServiceKey.SendName, receiver.GetArgs<string>(ServiceKey.SendName));
                SendCommand(sender);
            }
            else
                Server?.RelayCommand(receiver);
        }
        else if (operateCode is OperateCode.Callback)
        {
            if (Player?.Name == receiver.GetArgs<string>(ServiceKey.SendName))
            {
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback, receiver.Data, 0, receiver.Data.Length)
                    .AppendArgs(ServiceKey.ReceiveName, receiver.GetArgs<string>(ServiceKey.ReceiveName))
                    .AppendArgs(ServiceKey.SendName, receiver.GetArgs<string>(ServiceKey.SendName));
                CallbackSuccess(sender);
            }
            else
            {
                ReceiveCallback(receiver);
                Server?.RelayCommand(receiver);
            }
        }
        else if (operateCode is OperateCode.Broadcast)
        {
            ReceiveCallback(receiver);
            var message = FormatMessage(receiver);
            this.HandleLog(message);
        }
    }

    public void UpdatePlayerList(string[] playerList)
    {
        try
        {
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Player, (byte)OperateCode.List)
                .AppendArgs(ServiceKey.List, playerList);
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
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Archive, (byte)OperateCode.List)
                .AppendArgs(ServiceKey.List, Atlas.Archives.ToArray());
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
        if (operateCode is OperateCode.List)
        {
            ReceiveCallback(receiver);
        }
        else if (operateCode is OperateCode.Request)
        {
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            if (Atlas.GetPlayerArchive(Player.Name, out var archive))
            {
                sender.AppendArgs(ServiceKey.Archive, archive);
                CallbackSuccess(sender);
            }
            else
                CallbackFailure(sender, new NetException(ServiceCode.ServerNotStartYet));
        }
        else if (operateCode is OperateCode.Join)
        {
            OnJoinArchive?.Invoke(receiver);
        }
        else if (operateCode is OperateCode.Callback)
        {
            Joined = true;
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackSuccess(sender);
        }
    }

    public void ResbonseArchiveRequestOrJoin(CommandReceiver receiver, PlayerArchive playerArchive)
    {
        try
        {
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                .AppendArgs(ServiceKey.Archive, playerArchive);
            CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void UpdateCurrentDate(SpanFlowTickOnArgs args)
    {
        try
        {
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.SpanFlow, (byte)OperateCode.Update)
                .AppendArgs(ServiceKey.Args, args);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void HandleTimer(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Update)
        {
            ReceiveCallback(receiver);
        }
    }

    private void HandleLand(CommandReceiver receiver)
    {

        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Check)
        {
            var site = receiver.GetArgs<Coordinate>(ServiceKey.Site);
            var types = Atlas.GetCanBuildTypes(site);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                .AppendArgs(ServiceKey.Args, new SourceLandCanBuildArgs(site, types));
            CallbackSuccess(sender);
        }
        else if (operateCode is OperateCode.Update)
        {
            var site = receiver.GetArgs<Coordinate>(ServiceKey.Site);
            var type = receiver.GetArgs<SourceLandTypes>(ServiceKey.Type);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            if (!Server.BuildLand(site, type, Player.Name, out var vision))
                CallbackFailure(sender, new MapException(Localize.Table.BuildSourceLandFailed));
            else
            {
                sender.AppendArgs(ServiceKey.Object, vision);
                CallbackSuccess(sender);
            }
        }
    }
}
