﻿using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using WarringStates.Net.Common;
using WarringStates.Server.User;
using WarringStates.User;

namespace WarringStates.Server.Net;

partial class ServerService
{
    public NetEventHandler<CommandReceiver>? OnRelayCommand;

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
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Message, (byte)OperateCode.Broadcast, data, 0, count)
                .AppendArgs(ServiceKey.ReceiveUser, UserInfo.Name)
                .AppendArgs(ServiceKey.SendUser, nameof(Server));
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
            if (UserInfo?.Name == receiver.GetArgs(ServiceKey.ReceiveUser))
            {
                HandleMessage(receiver);
                var message = receiver.Data;
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Request, message, 0, message.Length)
                    .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                    .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
                SendCommand(sender);
            }
            else
                OnRelayCommand?.Invoke(receiver);
        }
        else if (operateCode is OperateCode.Callback)
        {
            if (UserInfo?.Name == receiver.GetArgs(ServiceKey.SendUser))
            {
                var message = receiver.Data;
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Callback, message, 0, message.Length)
                    .AppendArgs(ServiceKey.ReceiveUser, receiver.GetArgs(ServiceKey.ReceiveUser))
                    .AppendArgs(ServiceKey.SendUser, receiver.GetArgs(ServiceKey.SendUser));
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
            HandleMessage(receiver);
        }
    }

    public void UpdateUserList(string[] userList)
    {
        var count = WriteU8Buffer(userList.ToArrayString(), out var data);
        var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UpdateUserList, (byte)OperateCode.None, data, 0, count);
        SendCommand(sender);
    }

    private void DoArchive(CommandReceiver receiver)
    {
        var operateCode = (OperateCode)receiver.OperateCode;
        if (operateCode is OperateCode.Fetch)
        {
            var archiverInfoList = new List<PlayerArchiveInfo>();
            foreach (var info in LocalArchives.ReLocate())
            {
                var players = Archive.LoadPlayers(info);
                var sourceLands = Archive.LoadSourceLands(info);
                if (!sourceLands.TryGetValue(UserInfo.Id, out var ownLands))
                    ownLands = [];
                var archiveInfo = new PlayerArchiveInfo(info.Id, info.WorldName, info.WorldSize, players.Count, ownLands);
                archiverInfoList.Add(archiveInfo);
            }
            var data = archiverInfoList.ToSsBuffer(ServiceKey.ArchiveList);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode, data, 0, data.Length);
            CallbackSuccess(sender);
        }
        if (operateCode is OperateCode.Join)
        {
            // TODO: join archive
        }
    }
}