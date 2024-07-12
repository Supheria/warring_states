using LocalUtilities.IocpNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Net.Common;

namespace WarringStates.Client.Net;

partial class ClientService
{
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
                .AppendArgs(ServiceKey.ReceiveUser, UserInfo?.Name ?? "")
                //.AppendArgs(ServiceKey.SendUser, sendUser);
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
        try
        {
            switch ((OperateCode)receiver.OperateCode)
            {
                case OperateCode.Request:
                    HandleMessage(receiver);
                    var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback)
                        .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
                    CallbackSuccess(sender);
                    break;
                case OperateCode.Callback:
                    ReceiveCallback(receiver);
                    HandleMessage(receiver);
                    break;

            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback);
            CallbackFailure(sender, ex);
        }
    }
}
