using LocalUtilities.IocpNet.Common;
using LocalUtilities.IocpNet.Transfer;
using LocalUtilities.IocpNet.Transfer.Packet;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.Text;
using System.Net.Sockets;
using System.Text;

namespace WarringStates.Net.Model;

public class ServerService : Service
{
    public ServiceTypes Type { get; private set; } = ServiceTypes.None;

    protected override string RepoPath { get; set; } = @"repo\server";

    public string TimeStamp { get; } = DateTime.Now.ToString(DateTimeFormat.Data);

    public ServerService() : base(new ServerProtocol())
    {
        DaemonThread = new(ConstTabel.SocketTimeoutMilliseconds, CheckTimeout);
        Protocol.OnReceiveCommand += Protocol_OnReceiveCommand;
    }

    public override string GetLog(string log)
    {
        return new StringBuilder()
            .Append(SignTable.OpenBracket)
            .Append(Protocol.SocketInfo.RemoteEndPoint)
            .Append(SignTable.Colon)
            .Append(SignTable.Space)
            .Append(Type)
            .Append(SignTable.CloseBracket)
            .Append(SignTable.Space)
            .Append(log)
            .ToString();
    }

    public void Accept(Socket acceptSocket)
    {
        ((ServerProtocol)Protocol).Accept(acceptSocket);
        DaemonThread?.Start();
    }

    private void CheckTimeout()
    {
        var span = DateTime.Now - Protocol.SocketInfo.ActiveTime;
        if (span.TotalMilliseconds < ConstTabel.SocketTimeoutMilliseconds)
            return;
        Dispose();
    }

    private void Protocol_OnReceiveCommand(CommandReceiver receiver)
    {
        try
        {
            var commandCode = (CommandCode)receiver.CommandCode;
            if (commandCode is not CommandCode.Login && !IsLogined)
                throw new IocpException(ServiceCode.NotLogined);
            switch (commandCode)
            {
                case CommandCode.Login:
                    DoLogin(receiver);
                    break;
                case CommandCode.HeartBeats:
                    DoHeartBeats(receiver);
                    break;
                case CommandCode.TransferFile:
                    DoTransferFile(receiver);
                    break;
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

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
                throw new IocpException(ServiceCode.UserAlreadyLogined);
            var name = receiver.GetArgs(ServiceKey.UserName);
            var password = receiver.GetArgs(ServiceKey.Password);
            var type = receiver.GetArgs(ServiceKey.ProtocolType).ToEnum<ServiceTypes>();
            if (type is ServiceTypes.None || Type is not ServiceTypes.None && Type != type)
                throw new IocpException(ServiceCode.WrongProtocolType);
            Type = type;
            if (type is not ServiceTypes.HeartBeats)
            {
                DaemonThread?.Dispose();
                DaemonThread = null;
            }
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

    public void DoTransferFile(CommandReceiver receiver)
    {
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.UploadRequest:
                DoUploadRequestAsync(receiver);
                break;
            case OperateCode.UploadContinue:
                DoUploadContinue(receiver);
                break;
            case OperateCode.DownloadRequest:
                DoDownloadRequestAsync(receiver);
                break;
            case OperateCode.DownloadContinue:
                DoDownloadContinue(receiver);
                break;
            case OperateCode.DownloadFinish:
                DoDownloadFinish(receiver);
                break;
        }
    }

    private async void DoUploadRequestAsync(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            var filePath = GetFileRepoPath(fileArgs.DirName, fileArgs.FileName);
            if (File.Exists(filePath))
            {
                var task = Task.Run(() =>
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return fileStream.ToMd5HashString();
                });
                if (await task == fileArgs.Md5Value)
                    throw new IocpException(ServiceCode.SameVersionAlreadyExist);
                File.Delete(filePath);
            }
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            if (!AutoFile.Relocate(fileStream))
                throw new IocpException(ServiceCode.ProcessingFile);
            HandleUploadStart();
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackFailure(sender, ex);
        }
    }

    private void DoUploadContinue(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            if (AutoFile.IsExpired)
                throw new IocpException(ServiceCode.FileExpired, fileArgs.FileName);
            AutoFile.Write(receiver.Data);
            // simple validation
            if (AutoFile.Position != fileArgs.FilePosition)
                throw new IocpException(ServiceCode.NotSameVersion);
            if (AutoFile.Position < fileArgs.FileLength)
            {
                HandleUploading(fileArgs.FileLength, AutoFile.Position);
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                    .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
                CallbackSuccess(sender);
            }
            else
            {
                AutoFile.Dispose();
                HandleUploaded(fileArgs.StartTime);
                var startTime = BitConverter.GetBytes(fileArgs.StartTime.ToBinary());
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.UploadFinish, startTime, 0, startTime.Length);
                CallbackSuccess(sender);
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackFailure(sender, ex);
        }
    }

    private async void DoDownloadRequestAsync(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            var filePath = GetFileRepoPath(fileArgs.DirName, fileArgs.FileName);
            if (!File.Exists(filePath))
                throw new IocpException(ServiceCode.FileNotExist, filePath);
            var task = Task.Run(() =>
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fileStream.ToMd5HashString();
            });
            if (await task == fileArgs.Md5Value)
                throw new IocpException(ServiceCode.SameVersionAlreadyExist);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!AutoFile.Relocate(fileStream))
                throw new IocpException(ServiceCode.ProcessingFile);
            HandleDownloadStart();
            fileArgs.FileLength = AutoFile.Length;
            fileArgs.PacketLength = AutoFile.Length > ConstTabel.DataBytesTransferredMax ? ConstTabel.DataBytesTransferredMax : AutoFile.Length;
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackFailure(sender, ex);
        }
    }

    private void DoDownloadContinue(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            if (AutoFile.IsExpired)
                throw new IocpException(ServiceCode.FileExpired, fileArgs.FileName);
            AutoFile.Position = fileArgs.FilePosition;
            var data = new byte[fileArgs.PacketLength];
            if (!AutoFile.Read(data, out var count))
                throw new IocpException(ServiceCode.FileExpired, fileArgs.FileName);
            HandleDownloading(AutoFile.Length, AutoFile.Position);
            fileArgs.FilePosition = AutoFile.Position;
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode, data, 0, count)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackFailure(sender, ex);
        }
    }

    private void DoDownloadFinish(CommandReceiver receiver)
    {
        try
        {
            AutoFile.Dispose();
            var startTime = DateTime.FromBinary(BitConverter.ToInt64(receiver.Data));
            HandleDownloaded(startTime);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode);
            CallbackFailure(sender, ex);
        }
    }

    //private void DoDir(CommandParser commandParser)
    //{
    //    if (!commandParser.GetValueAsString(ProtocolKey.ParentDir, out var dir))
    //    {
    //        CommandFail(ProtocolCode.ParameterError, "");
    //        return;
    //    }
    //    if (!Directory.Exists(dir))
    //    {
    //        CommandFail(ProtocolCode.DirNotExist, dir);
    //        return;
    //    }
    //    char[] directorySeparator = [Path.DirectorySeparatorChar];
    //    try
    //    {
    //        var commandComposer = new CommandComposer()
    //            .AppendCommand(ProtocolKey.Dir);
    //        foreach (var subDir in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
    //        {
    //            var dirName = subDir.Split(directorySeparator, StringSplitOptions.RemoveEmptyEntries);
    //            commandComposer.AppendValue(ProtocolKey.Item, dirName[dirName.Length - 1]);

    //        }
    //        CommandSucceed(commandComposer);
    //    }
    //    catch (Exception ex)
    //    {
    //        CommandFail(ProtocolCode.UnknowError, ex.Message);
    //    }
    //}

    //private void DoFileList(CommandParser commandParser)
    //{
    //    if (!commandParser.GetValueAsString(ProtocolKey.DirName, out var dir))
    //    {
    //        CommandFail(ProtocolCode.ParameterError, "");
    //        return;
    //    }
    //    dir = dir is "" ? RootDirectoryPath : Path.Combine(RootDirectoryPath, dir);
    //    if (!Directory.Exists(dir))
    //    {
    //        CommandFail(ProtocolCode.DirNotExist, dir);
    //        return;
    //    }
    //    try
    //    {
    //        var commandComposer = new CommandComposer()
    //            .AppendCommand(ProtocolKey.FileList);
    //        foreach (var file in Directory.GetFiles(dir))
    //        {
    //            var fileInfo = new FileInfo(file);
    //            commandComposer.AppendValue(ProtocolKey.Item, fileInfo.Name + ProtocolKey.TextSeperator + fileInfo.Length.ToString());
    //        }
    //        CommandSucceed(commandComposer);
    //    }
    //    catch (Exception ex)
    //    {
    //        CommandFail(ProtocolCode.UnknowError, ex.Message);
    //    }
    //}
}
