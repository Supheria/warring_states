using LocalUtilities.IocpNet.Common;
using WarringStates.Client.User;
using WarringStates.Net.Common;
using WarringStates.User;
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeToolKit.Convert;
using LocalUtilities.SimpleScript.Common;

namespace WarringStates.Client.Net;

partial class ClientService
{
    public event NetEventHandler<string[]>? OnUpdatePlayerList;

    public void SendMessage(string message, string receivePlayerName)
    {
        try
        {
            var data = SerializeTool.Serialize(message, new(), SignTable, null);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Request, data, 0, data.Length)
                .AppendArgs(ServiceKey.ReceivePlayer, receivePlayerName)
                .AppendArgs(ServiceKey.SendPlayer, Player.Name);
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
    }

    public void JoinArchive(string id)
    {
        var data = SerializeTool.Serialize(id, new(), SignTable, null);
        var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Archive, (byte)OperateCode.Join, data, 0, data.Length);
        SendCommand(sender);
    }
}
