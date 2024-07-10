using LocalUtilities.IocpNet.Common;
using LocalUtilities.IocpNet.Protocol;
using LocalUtilities.IocpNet.Transfer.Packet;
using LocalUtilities.TypeGeneral;
using System.Collections.Concurrent;

namespace WarringStates.Net.Model;

public class ServerHost : Host
{
    public NetEventHandler<CommandReceiver>? OnOperate;

    public NetEventHandler? OnClearUp;

    ConcurrentDictionary<ServiceTypes, ServerService> Protocols { get; } = [];

    public string UserName => UserInfo?.Name ?? "";

    public int Count => Protocols.Count;

    public bool Add(ServerService service)
    {
        if (service.UserInfo is null || UserInfo is not null && service.UserInfo != UserInfo)
            goto CLOSE;
        UserInfo = service.UserInfo;
        if (Protocols.TryGetValue(service.Type, out var toCheck) && toCheck.TimeStamp != service.TimeStamp)
            goto CLOSE;
        if (!Protocols.TryAdd(service.Type, service))
            goto CLOSE;
        if (service.Type is ServiceTypes.Operator)
        {
            service.OnOperate += DoOperate;
            service.OnOperateCallback += ReceiveOperateCallback;
        }
        else if (service.Type is ServiceTypes.HeartBeats)
            service.OnClosed += CloseAll;
        return true;
    CLOSE:
        service.Dispose();
        return false;
    }

    public void Remove(ServerService service)
    {
        if (Protocols.TryGetValue(service.Type, out var toCheck) && toCheck.TimeStamp == service.TimeStamp)
            Protocols.TryRemove(service.Type, out _);
        if (Protocols.Count is 0)
            OnClearUp?.Invoke();
    }

    public void CloseAll()
    {
        foreach (var protocol in Protocols.Values)
        {
            lock (protocol)
                protocol.Dispose();
        }
    }

    public void DoOperate(CommandReceiver receiver)
    {
        var sender = new CommandSender(receiver.TimeStamp, (byte)CommandCode.OperateCallback, receiver.OperateCode);
        try
        {
            switch ((OperateCode)receiver.OperateCode)
            {
                case OperateCode.Message:
                    DoMessage(receiver);
                    break;
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            if (Protocols.TryGetValue(ServiceTypes.Operator, out var protocol))
                protocol.CallbackFailure(sender, ex);
        }
    }

    private void DoMessage(CommandReceiver receiver)
    {
        var userName = receiver.GetArgs(ServiceKey.ReceiveUser);
        if (userName != UserName)
        {
            OnOperate?.Invoke(receiver);
            var sender = new CommandSender(receiver.TimeStamp, (byte)CommandCode.OperateCallback, receiver.OperateCode);
            Protocols[ServiceTypes.Operator].CallbackSuccess(sender);
        }
        else
        {
            HandleMessage(receiver);
            var data = receiver.Data;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Operate, receiver.OperateCode, data, 0, data.Length)
                .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
            Protocols[ServiceTypes.Operator].SendCommand(sender);
        }
        // TODO: make callback by receive user's client
    }

    private void ReceiveOperateCallback(CommandReceiver receiver)
    {

    }

    public void SendMessage(string message)
    {
        try
        {
            var count = WriteU8Buffer(message, out var data);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Operate, (byte)OperateCode.Message, data, 0, count)
                .AppendArgs(ServiceKey.ReceiveUser, UserName)
                .AppendArgs(ServiceKey.SendUser, "Host");
            Protocols[ServiceTypes.Operator].SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void UpdateUserList(string[] userList)
    {
        try
        {
            var count = WriteU8Buffer(userList.ToArrayString(), out var data);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Operate, (byte)OperateCode.UpdateUserList, data, 0, count);
            Protocols[ServiceTypes.Operator].SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }
}
