using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Net.Sockets;
using System.Text;
using WarringStates.Net;
using WarringStates.Net.Common;
using WarringStates.Net.Utilities;

namespace WarringStates.Server.Net;

internal partial class ServerService : Service
{
    protected override string RepoPath { get; set; } = @"repo\server";

    public string TimeStamp { get; } = DateTime.Now.ToString(DateTimeFormat.Data);

    protected override DaemonThread DaemonThread { get; init; }

    public ServerService() : base(new ServerProtocol())
    {
        DaemonThread = new(ConstTabel.SocketTimeoutMilliseconds, CheckTimeout);
        DoCommands[CommandCode.HeartBeats] = DoHeartBeats;
        DoCommands[CommandCode.Login] = DoLogin;
        DoCommands[CommandCode.UploadFile] = DoUploadFile;
        DoCommands[CommandCode.DownloadFile] = DoDownloadFile;
        DoCommands[CommandCode.Message] = DoMessage;
        DoCommands[CommandCode.UpdateUserList] = ReceiveCallback;
        DoCommands[CommandCode.Archive] = DoArchive;
    }

    public override void DoCommand(CommandReceiver receiver)
    {
        try
        {
            var commandCode = (CommandCode)receiver.CommandCode;
            if (commandCode is not CommandCode.Login && !IsLogined)
                throw new NetException(ServiceCode.NotLogined);
            if (!DoCommands.TryGetValue(commandCode, out var doCommand))
                throw new NetException(ServiceCode.UnknownCommand, commandCode.ToString());
            doCommand(receiver);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public override string GetLog(string message)
    {
        return new StringBuilder()
            .Append(UserInfo?.Name)
            .Append(SignTable.Colon)
            .Append(SignTable.Space)
            .Append(SignTable.OpenBracket)
            .Append(Protocol.SocketInfo.RemoteEndPoint)
            .Append(SignTable.CloseBracket)
            .Append(SignTable.Space)
            .Append(message)
            .Append(SignTable.Space)
            .Append(SignTable.At)
            .Append(DateTime.Now.ToString(DateTimeFormat.Outlook))
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
