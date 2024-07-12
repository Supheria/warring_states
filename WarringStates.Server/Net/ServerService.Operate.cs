using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using WarringStates.Net.Common;

namespace WarringStates.Server.Net;

partial class ServerService
{
    public NetEventHandler<CommandReceiver>? OnCommand;

    private void DoHeartBeats(CommandReceiver receiver)
    {
        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
        CallbackSuccess(sender);
    }

    private void DoLogin(CommandReceiver receiver)
    {
        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
        try
        {
            if (IsLogined)
                throw new NetException(ServiceCode.UserAlreadyLogined);
            var name = receiver.GetArgs(ServiceKey.UserName);
            var password = receiver.GetArgs(ServiceKey.Password);
            // TODO: validate userinfo
            UserInfo = new(name, password);
            IsLogined = true;
            HandleLogined();
            CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            CallbackFailure(sender, ex);
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            var count = WriteU8Buffer(message, out var data);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Request, data, 0, count)
                .AppendArgs(ServiceKey.ReceiveUser, UserInfo?.Name ?? "")
                .AppendArgs(ServiceKey.SendUser, "Server");
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoMessage(CommandReceiver receiver)
    {
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.Request:
                try
                {
                    var userName = receiver.GetArgs(ServiceKey.ReceiveUser);
                    if (userName != UserInfo?.Name)
                    {
                        OnCommand?.Invoke(receiver);
                        var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback)
                            .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                            .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
                        CallbackSuccess(sender);
                    }
                    else
                    {
                        HandleMessage(receiver);
                        var data = receiver.Data;
                        var sender = new CommandSender(DateTime.Now, receiver.CommandCode, (byte)OperateCode.Request, data, 0, data.Length)
                            .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                            .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
                        SendCommand(sender);
                    }
                }
                catch (Exception ex)
                {
                    this.HandleException(ex);
                    var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback);
                    CallbackFailure(sender, ex);
                }
                break;
            case OperateCode.Callback:
                try
                {
                    ReceiveCallback(receiver);
                    HandleMessage(receiver);
                }
                catch (Exception ex)
                {
                    this.HandleException(ex);
                }
                break;
        }
    }

    public void UpdateUserList(string[] userList)
    {
        try
        {
            var count = WriteU8Buffer(userList.ToArrayString(), out var data);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UpdateUserList, (byte)OperateCode.None, data, 0, count);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }
}
