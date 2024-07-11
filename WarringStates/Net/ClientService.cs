using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Net;
using System.Text;
using WarringStates.Net.Common;
using WarringStates.User;

namespace WarringStates.Net;

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

    private void Protocol_OnReceiveCommand(CommandReceiver receiver)
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
            switch ((CommandCode)receiver.CommandCode)
            {
                case CommandCode.Login:
                    DoLogin(receiver);
                    break;
                case CommandCode.HeartBeats:
                    DoHeartBeats(receiver);
                    break;
                case CommandCode.UploadFile:
                    DoUploadFile(receiver);
                    break;
                case CommandCode.DownloadFile:
                    DoDownloadFile(receiver);
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
                throw new NetException(ServiceCode.EmptyUserInfo);
            if (!((ClientProtocol)Protocol).Connect(host))
                throw new NetException(ServiceCode.NoConnection);
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
                throw new NetException(ServiceCode.ProcessingFile);
            var filePath = GetFileRepoPath(dirName, fileName);
            if (!File.Exists(filePath))
                throw new NetException(ServiceCode.FileNotExist, filePath);
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
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UploadFile, (byte)OperateCode.Request)
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
                throw new NetException(ServiceCode.ProcessingFile);
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
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Request)
                .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void DoUploadFile(CommandReceiver receiver)
    {
        ReceiveCallback(receiver);
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.Request:
                DoUploadRequest(receiver);
                break;
            case OperateCode.Continue:
                DoUploadContinue(receiver);
                break;
            case OperateCode.Finish:
                DoUploadFinish(receiver);
                break;
        }
    }
    private void DoDownloadFile(CommandReceiver receiver)
    {
        ReceiveCallback(receiver);
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.Request:
                DoDownloadRequest(receiver);
                break;
            case OperateCode.Continue:
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
                throw new NetException(ServiceCode.ProcessingFile);
            fileArgs.FileLength = AutoFile.Length;
            fileArgs.PacketLength = AutoFile.Length > ConstTabel.DataBytesTransferredMax ? ConstTabel.DataBytesTransferredMax : AutoFile.Length;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UploadFile, (byte)OperateCode.Continue)
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
                throw new NetException(ServiceCode.FileExpired, fileArgs.FileName);
            var data = new byte[fileArgs.PacketLength];
            if (!AutoFile.Read(data, out var count))
                throw new NetException(ServiceCode.FileExpired, fileArgs.FileName);
            HandleUploading(AutoFile.Length, AutoFile.Position);
            fileArgs.FileLength = AutoFile.Length;
            fileArgs.FilePosition = AutoFile.Position;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UploadFile, (byte)OperateCode.Continue, data, 0, count)
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
                throw new NetException(ServiceCode.ProcessingFile);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Continue)
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
                throw new NetException(ServiceCode.FileExpired, fileArgs.FileName);
            AutoFile.Write(receiver.Data);
            // simple validation
            if (AutoFile.Position != fileArgs.FilePosition)
                throw new NetException(ServiceCode.NotSameVersion);
            fileArgs.FilePosition = AutoFile.Position;
            if (AutoFile.Position < fileArgs.FileLength)
            {
                HandleDownloading(fileArgs.FileLength, AutoFile.Position);
                var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Continue)
                    .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
                SendCommand(sender);
            }
            else
            {
                AutoFile.Dispose();
                HandleDownloaded(fileArgs.StartTime);
                var startTime = BitConverter.GetBytes(fileArgs.StartTime.ToBinary());
                var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Finish, startTime, 0, startTime.Length);
                SendCommand(sender);
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }
}
