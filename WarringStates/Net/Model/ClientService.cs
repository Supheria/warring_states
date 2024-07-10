using LocalUtilities.IocpNet.Common;
using LocalUtilities.IocpNet.Transfer;
using LocalUtilities.IocpNet.Transfer.Packet;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WarringStates.Net.Model;

public class ClientService : Service
{
    public ServiceTypes Type { get; }

    protected override string RepoPath { get; set; } = @"repo\client";

    AutoResetEvent LoginDone { get; } = new(false);

    public ClientService(ServiceTypes type) : base(new ClientProtocol())
    {
        Type = type;
        if (type is ServiceTypes.HeartBeats)
            DaemonThread = new(ConstTabel.HeartBeatsInterval, HeartBeats);
        Protocol.OnReceiveCommand += Protocol_OnReceiveCommand;
        //Commands[CommandTypes.HeartBeats] = DoHeartBeats;
        //Commands[CommandTypes.Login] = DoLogin;
        //Commands[CommandTypes.TransferFile] = DoTransferFile;
    }

    public override string GetLog(string log)
    {
        return new StringBuilder()
                .Append(SignTable.OpenBracket)
                .Append(Protocol.SocketInfo.LocalEndPoint)
                .Append(SignTable.Colon)
                .Append(SignTable.Space)
                .Append(Type)
                .Append(SignTable.CloseBracket)
                .Append(SignTable.Space)
                .Append(log)
                .ToString();
    }

    private void Protocol_OnReceiveCommand(CommandReceiver reciver)
    {
        try
        {
            //if (!commandParser.GetValueAsString(ProtocolKey.Code, out var errorCode))
            //    throw new IocpException(ProtocolCode.UnknowError);
            //var code = errorCode.ToEnum<ProtocolCode>();
            //if (code is not ProtocolCode.Success)
            //{
            //    if (commandParser.GetValueAsString(ProtocolKey.Message, out var message))
            //        throw new IocpException(code, message);
            //    else
            //        throw new IocpException(code);
            //}
            switch ((CommandCode)reciver.CommandCode)
            {
                case CommandCode.Login:
                    DoLogin(reciver);
                    break;
                case CommandCode.HeartBeats:
                    DoHeartBeats(reciver);
                    break;
                case CommandCode.TransferFile:
                    DoTransferFile(reciver);
                    break;
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void HeartBeats()
    {
        try
        {
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.HeartBeats, (byte)OperateCode.None);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void Login(IPEndPoint? host, UserInfo? userInfo)
    {
        try
        {
            if (IsLogined)
                return;
            if (userInfo is null)
                throw new IocpException(ServiceCode.EmptyUserInfo);
            if (!((ClientProtocol)Protocol).Connect(host))
                throw new IocpException(ServiceCode.NoConnection);
            UserInfo = userInfo;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Login, (byte)OperateCode.None)
                .AppendArgs(ServiceKey.UserName, userInfo.Name ?? "")
                .AppendArgs(ServiceKey.Password, userInfo.Password)
                .AppendArgs(ServiceKey.ProtocolType, Type.ToString());
            SendCommand(sender);
            LoginDone?.WaitOne(ConstTabel.BlockkMilliseconds);
        }
        catch (Exception ex)
        {
            Dispose();
            this.HandleException(ex);
        }
    }

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

    //protected async Task<string> FileValdate(string filePath)
    //{
    //    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    //    var md5 = fileStream.ToMd5HashString();
    //    fileStream.Dispose();
    //    return md5;
    //}

    public async void UploadAsync(string dirName, string fileName)
    {
        try
        {
            if (!AutoFile.IsExpired)
                throw new IocpException(ServiceCode.ProcessingFile);
            var filePath = GetFileRepoPath(dirName, fileName);
            if (!File.Exists(filePath))
                throw new IocpException(ServiceCode.FileNotExist, filePath);
            var task = Task.Run(() =>
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fileStream.ToMd5HashString();
            });
            var fileArgs = new FileTransferArgs(dirName, fileName)
            {
                Md5Value = await task,
            };
            HandleUploadStart();
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.UploadRequest)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public async void DownLoadAsync(string dirName, string fileName)
    {
        try
        {
            if (!AutoFile.IsExpired)
                throw new IocpException(ServiceCode.ProcessingFile);
            var filePath = GetFileRepoPath(dirName, fileName);
            var task = Task.Run(() =>
            {
                using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                return fileStream.ToMd5HashString();
            });
            var fileArgs = new FileTransferArgs(dirName, fileName)
            {
                Md5Value = await task,
            };
            HandleDownloadStart();
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.DownloadRequest)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoTransferFile(CommandReceiver receiver)
    {
        ReceiveCallback(receiver);
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.UploadRequest:
                DoUploadRequest(receiver);
                break;
            case OperateCode.UploadContinue:
                DoUploadContinue(receiver);
                break;
            case OperateCode.UploadFinish:
                DoUploadFinish(receiver);
                break;
            case OperateCode.DownloadRequest:
                DoDownloadRequest(receiver);
                break;
            case OperateCode.DownloadContinue:
                DoDownloadContinue(receiver);
                break;
        }
    }

    private void DoUploadRequest(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            var filePath = GetFileRepoPath(fileArgs.DirName, fileArgs.FileName);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!AutoFile.Relocate(fileStream))
                throw new IocpException(ServiceCode.ProcessingFile);
            fileArgs.FileLength = AutoFile.Length;
            fileArgs.PacketLength = AutoFile.Length > ConstTabel.DataBytesTransferredMax ? ConstTabel.DataBytesTransferredMax : AutoFile.Length;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.UploadContinue)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoUploadContinue(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            if (AutoFile.IsExpired)
                throw new IocpException(ServiceCode.FileExpired, fileArgs.FileName);
            var data = new byte[fileArgs.PacketLength];
            if (!AutoFile.Read(data, out var count))
                throw new IocpException(ServiceCode.FileExpired, fileArgs.FileName);
            HandleUploading(AutoFile.Length, AutoFile.Position);
            fileArgs.FileLength = AutoFile.Length;
            fileArgs.FilePosition = AutoFile.Position;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.UploadContinue, data, 0, count)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoUploadFinish(CommandReceiver receiver)
    {
        try
        {
            AutoFile.Dispose();
            var startTime = DateTime.FromBinary(BitConverter.ToInt64(receiver.Data));
            HandleUploaded(startTime);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoDownloadRequest(CommandReceiver receiver)
    {
        try
        {
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            var filePath = GetFileRepoPath(fileArgs.DirName, fileArgs.FileName);
            File.Delete(filePath);
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            if (!AutoFile.Relocate(fileStream))
                throw new IocpException(ServiceCode.ProcessingFile);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.DownloadContinue)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoDownloadContinue(CommandReceiver receiver)
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
            fileArgs.FilePosition = AutoFile.Position;
            if (AutoFile.Position < fileArgs.FileLength)
            {
                HandleDownloading(fileArgs.FileLength, AutoFile.Position);
                var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.DownloadContinue)
                    .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
                SendCommand(sender);
            }
            else
            {
                AutoFile.Dispose();
                HandleDownloaded(fileArgs.StartTime);
                var startTime = BitConverter.GetBytes(fileArgs.StartTime.ToBinary());
                var sender = new CommandSender(DateTime.Now, (byte)CommandCode.TransferFile, (byte)OperateCode.DownloadFinish, startTime, 0, startTime.Length);
                SendCommand(sender);
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }
}
