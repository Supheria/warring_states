using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Net.Common;
using WarringStates.Net.Utilities;

namespace WarringStates.Client.Net;

partial class ClientService
{
    public async void UploadFileAsync(string dirName, string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new NetException(ServiceCode.FileNotExist, filePath);
            var task = Task.Run(() =>
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return HashTool.ToMd5HashString(fileStream);
            });
            var fileArgs = new FileTransferArgs(dirName, filePath)
            {
                Md5Value = await task,
            };
            HandleUploadStart();
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UploadFile, (byte)OperateCode.Request)
                .AppendArgs(ServiceKey.Args, fileArgs);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public async void DownLoadFileAsync(string dirName, string filePath)
    {
        try
        {
            var task = Task.Run(() =>
            {
                using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                return HashTool.ToMd5HashString(fileStream);
            });
            var fileArgs = new FileTransferArgs(dirName, filePath)
            {
                Md5Value = await task,
            };
            HandleDownloadStart();
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Request)
                .AppendArgs(ServiceKey.Args, fileArgs);
            SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    private void HandleUploadFile(CommandReceiver receiver)
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
    private void HandleDownloadFile(CommandReceiver receiver)
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
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.Args) ?? throw new NetException(ServiceCode.MissingCommandArgs, nameof(FileTransferArgs));
            var fileStream = new FileStream(fileArgs.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var autoFile = new AutoDisposeFileStream(fileStream, fileArgs.StartTime);
            if (!AutoFiles.TryAdd(autoFile))
                throw new NetException(ServiceCode.CannotAddFileToProcess, fileArgs.FilePath);
            fileArgs.FileLength = autoFile.Length;
            fileArgs.PacketLength = autoFile.Length > DataLengthMax ? DataLengthMax : autoFile.Length;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UploadFile, (byte)OperateCode.Continue)
                .AppendArgs(ServiceKey.Args, fileArgs);
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
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.Args) ?? throw new NetException(ServiceCode.MissingCommandArgs, nameof(FileTransferArgs));
            if (!AutoFiles.TryGetValue(fileArgs.StartTime, out var autoFile))
                throw new NetException(ServiceCode.FileExpired, fileArgs.FilePath);
            var data = new byte[fileArgs.PacketLength];
            autoFile.Read(data, out var count);
            HandleUploading(autoFile.Length, autoFile.Position);
            fileArgs.FileLength = autoFile.Length;
            fileArgs.FilePosition = autoFile.Position;
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.UploadFile, (byte)OperateCode.Continue, data, 0, count)
                .AppendArgs(ServiceKey.Args, fileArgs);
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
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.Args) ?? throw new NetException(ServiceCode.MissingCommandArgs, nameof(FileTransferArgs));
            if (!AutoFiles.TryGetValue(fileArgs.StartTime, out var autoFile))
                throw new NetException(ServiceCode.FileExpired, fileArgs.FilePath);
            autoFile.Dispose();
            HandleUploaded(fileArgs.StartTime);
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
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.Args) ?? throw new NetException(ServiceCode.MissingCommandArgs, nameof(FileTransferArgs));
            File.Delete(fileArgs.FilePath);
            var fileStream = new FileStream(fileArgs.FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            var autoFile = new AutoDisposeFileStream(fileStream, fileArgs.StartTime);
            autoFile.OnDisposed += () => this.HandleLog(AutoFiles.Count.ToString());
            if (!AutoFiles.TryAdd(autoFile))
                throw new NetException(ServiceCode.CannotAddFileToProcess, fileArgs.FilePath);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Continue)
                .AppendArgs(ServiceKey.Args, fileArgs);
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
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.Args) ?? throw new NetException(ServiceCode.MissingCommandArgs, nameof(FileTransferArgs));
            if (!AutoFiles.TryGetValue(fileArgs.StartTime, out var autoFile))
                throw new NetException(ServiceCode.FileExpired, fileArgs.FilePath);
            autoFile.Write(receiver.Data);
            // simple validation
            if (autoFile.Position != fileArgs.FilePosition)
                throw new NetException(ServiceCode.NotSameVersion);
            fileArgs.FilePosition = autoFile.Position;
            if (autoFile.Position < fileArgs.FileLength)
            {
                HandleDownloading(fileArgs.FileLength, autoFile.Position);
                var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Continue)
                    .AppendArgs(ServiceKey.Args, fileArgs);
                SendCommand(sender);
            }
            else
            {
                autoFile.Dispose();
                HandleDownloaded(fileArgs.StartTime);
                var startTime = BitConverter.GetBytes(fileArgs.StartTime.ToBinary());
                var sender = new CommandSender(DateTime.Now, (byte)CommandCode.DownloadFile, (byte)OperateCode.Finish)
                    .AppendArgs(ServiceKey.Args, fileArgs);
                SendCommand(sender);
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }
}
