﻿using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Net.Common;
using WarringStates.Net.Utilities;

namespace WarringStates.Server.Net;

partial class ServerService
{
    private void DoUploadFile(CommandReceiver receiver)
    {
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.Request:
                DoUploadRequestAsync(receiver);
                break;
            case OperateCode.Continue:
                DoUploadContinue(receiver);
                break;
        }
    }

    private void DoDownloadFile(CommandReceiver receiver)
    {
        switch ((OperateCode)receiver.OperateCode)
        {
            case OperateCode.Request:
                DoDownloadRequestAsync(receiver);
                break;
            case OperateCode.Continue:
                DoDownloadContinue(receiver);
                break;
            case OperateCode.Finish:
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
                    throw new NetException(ServiceCode.SameVersionAlreadyExist);
                File.Delete(filePath);
                filePath = filePath.RenamePathByDateTime();
            }
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            var autoFile = new AutoDisposeFileStream(fileStream, fileArgs.StartTime);
            if (!AutoFiles.TryAdd(autoFile))
                throw new NetException(ServiceCode.CannotAddFileToProcess, filePath);
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
            if (!AutoFiles.TryGetValue(fileArgs.StartTime, out var autoFile))
                throw new NetException(ServiceCode.FileExpired, GetFileRepoPath(fileArgs.DirName, fileArgs.FileName));
            autoFile.Write(receiver.Data);
            // simple validation
            if (autoFile.Position != fileArgs.FilePosition)
                throw new NetException(ServiceCode.NotSameVersion);
            if (autoFile.Position < fileArgs.FileLength)
            {
                HandleUploading(fileArgs.FileLength, autoFile.Position);
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, receiver.OperateCode)
                    .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
                CallbackSuccess(sender);
            }
            else
            {
                autoFile.Dispose();
                HandleUploaded(fileArgs.StartTime);
                var startTime = BitConverter.GetBytes(fileArgs.StartTime.ToBinary());
                var sender = new CommandSender(receiver.TimeStamp, receiver.CommandCode, (byte)OperateCode.Finish)
                    .AppendArgs(ServiceKey.FileTransferArgs, fileArgs.ToSsString());
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
                throw new NetException(ServiceCode.FileNotExist, filePath);
            var task = Task.Run(() =>
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fileStream.ToMd5HashString();
            });
            if (await task == fileArgs.Md5Value)
                throw new NetException(ServiceCode.SameVersionAlreadyExist);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var autoFile = new AutoDisposeFileStream(fileStream, fileArgs.StartTime);
            if (!AutoFiles.TryAdd(autoFile))
                throw new NetException(ServiceCode.CannotAddFileToProcess, filePath);
            HandleDownloadStart();
            fileArgs.FileLength = autoFile.Length;
            fileArgs.PacketLength = autoFile.Length > DataLengthMax ? DataLengthMax : autoFile.Length;
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
            if (!AutoFiles.TryGetValue(fileArgs.StartTime, out var autoFile))
                throw new NetException(ServiceCode.FileExpired, GetFileRepoPath(fileArgs.DirName, fileArgs.FileName));
            autoFile.Position = fileArgs.FilePosition;
            var data = new byte[fileArgs.PacketLength];
            autoFile.Read(data, out var count);
            HandleDownloading(autoFile.Length, autoFile.Position);
            fileArgs.FilePosition = autoFile.Position;
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
            var fileArgs = receiver.GetArgs<FileTransferArgs>(ServiceKey.FileTransferArgs);
            if (!AutoFiles.TryGetValue(fileArgs.StartTime, out var autoFile))
                throw new NetException(ServiceCode.FileExpired, GetFileRepoPath(fileArgs.DirName, fileArgs.FileName));
            autoFile.Dispose();
            HandleDownloaded(fileArgs.StartTime);
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
}