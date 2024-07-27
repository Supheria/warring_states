using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;
using WarringStates.Flow;
using WarringStates.Net.Common;
using WarringStates.Server.User;
using WarringStates.User;

namespace WarringStates.Server.Net;

partial class ServerService
{
    public event NetEventHandler<CommandReceiver>? OnRequestArchive;

    public event NetEventHandler<CommandReceiver>? OnJoinArchive;

    public PlayerGroup? PlayerGroup { get; set; } = null;

    private void HandleHeartBeats(CommandReceiver receiver)
    {
        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
        CallbackSuccess(sender);
    }

    private void HandleLogin(CommandReceiver receiver)
    {
        try
        {
            if (IsLogined)
                throw new NetException(ServiceCode.UserAlreadyLogined);
            var name = receiver.GetArgs<string>(ServiceKey.UserName);
            var password = receiver.GetArgs<string>(ServiceKey.Password);
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                throw new NetException(ServiceCode.MissingCommandArgs, ServiceKey.UserName, ServiceKey.Password);
            if (!LocalNet.CheckLogin(name, password, out var player, out var code))
                throw new NetException(code);
            Player = player;
            IsLogined = true;
            HandleLogined();
            var data = SerializeTool.Serialize(Player, new(), SignTable, null);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode, data, 0, data.Length);
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
                PlayerGroup?.RelayCommand(receiver);
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
                PlayerGroup?.RelayCommand(receiver);
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
            var data = SerializeTool.Serialize(playerList, new(), SignTable, null);
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
            var data = SerializeTool.Serialize(LocalArchive.Archives.ToArray(), new(), SignTable, null);
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
        if (operateCode is OperateCode.List)
        {
            ReceiveCallback(receiver);
        }
        else if (operateCode is OperateCode.Request)
        {
            OnRequestArchive?.Invoke(receiver);
        }
        else if (operateCode is OperateCode.Join)
        {
            OnJoinArchive?.Invoke(receiver);
        }
    }

    public void ResbonseArchiveRequestOrJoin(CommandReceiver receiver, PlayerArchive playerArchive)
    {
        try
        {
            var data = SerializeTool.Serialize(playerArchive, new(), SignTable, null);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode, data, 0, data.Length);
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
            var date = args.CurrentDate;
            var format = ArrayString.ToArrayString(date.Year, date.Month, date.Day, date.Type);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.SpanFlow, (byte)OperateCode.Update)
                .AppendArgs(ServiceKey.Date, format)
                .AppendArgs(ServiceKey.Span, args.CurrentSpan);
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
            var site = receiver.GetArgs<Coordinate>(ServiceKey.Site) ??
                throw new NetException(ServiceCode.MissingCommandArgs);
            var types = PlayerGroup!.LandMap.CanBuildTypes(site);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                .AppendArgs(ServiceKey.List, types);
            CallbackSuccess(sender);
        }
    }
}
