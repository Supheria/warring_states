using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using WarringStates.Net.Common;

namespace WarringStates.Client.Net;

partial class ClientService
{
    public event NetEventHandler<string[]>? OnUpdateUserList;

    private void DoHeartBeats(CommandReceiver receiver)
    {
        try
        {
            ReceiveCallback(receiver);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoLogin(CommandReceiver receiver)
    {
        try
        {
            ReceiveCallback(receiver);
            IsLogined = true;
            LoginDone.Set();
            HandleLogined();
            DaemonThread?.Start();
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void SendMessage(string message, string sendUser)
    {
        try
        {
            var count = WriteU8Buffer(message, out var data);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Request, data, 0, count)
                .AppendArgs(ServiceKey.ReceiveUser, sendUser)
                .AppendArgs(ServiceKey.SendUser, UserInfo?.Name ?? "");
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoMessage(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Request)
        {
            HandleMessage(receiver);
            var message = receiver.Data;
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback, message, 0, message.Length)
                    .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                    .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
            CallbackSuccess(sender);
        }
        else if (operateCode is OperateCode.Callback)
        {
            ReceiveCallback(receiver);
            HandleMessage(receiver);
        }
        else if (operateCode is OperateCode.Broadcast)
        {
            HandleMessage(receiver);
            var message = receiver.Data;
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode, message, 0, message.Length)
                    .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                    .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
            CallbackSuccess(sender);
        }
    }

    private void DoUpdateUserList(CommandReceiver receiver)
    {
        var userList = ReadU8Buffer(receiver.Data).ToArray();
        OnUpdateUserList?.Invoke(userList);
        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.None);
        CallbackSuccess(sender);
    }
}
