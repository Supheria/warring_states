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

internal partial class ServerService : NetService, IRosterItem<string>
{
    protected string RepoPath { get; set; } = @"repo\server";

    public string TimeStamp { get; } = DateTime.Now.ToString(DateTimeFormat.Data);

    protected override DaemonThread DaemonThread { get; init; }

    public string Signature => Player.Id;

    public ServerService() : base(new ServerProtocol())
    {
        DaemonThread = new(ConstTabel.SocketTimeoutMilliseconds, CheckTimeout);
        HandleCommands[CommandCode.HeartBeats] = HandleHeartBeats;
        HandleCommands[CommandCode.Login] = HandleLogin;
        HandleCommands[CommandCode.UploadFile] = HandleUploadFile;
        HandleCommands[CommandCode.DownloadFile] = HandleDownloadFile;
        HandleCommands[CommandCode.Message] = HandleMessage;
        HandleCommands[CommandCode.Player] = ReceiveCallback;
        HandleCommands[CommandCode.Archive] = HandleArchive;
    }

    public override void HandleCommand(CommandReceiver receiver)
    {
        try
        {
            var commandCode = (CommandCode)receiver.CommandCode;
            if (commandCode is not CommandCode.Login && !IsLogined)
                throw new NetException(ServiceCode.NotLogined);
            if (!HandleCommands.TryGetValue(commandCode, out var doCommand))
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
            .Append(Player.Name)
            .Append(SignCollection.Colon)
            .Append(SignCollection.Space)
            .Append(SignCollection.OpenBracket)
            .Append(Protocol.SocketInfo.RemoteEndPoint)
            .Append(SignCollection.CloseBracket)
            .Append(SignCollection.Space)
            .Append(message)
            .Append(SignCollection.Space)
            .Append(SignCollection.At)
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

    public string GetFileRepoPath(string dirName, string fileName)
    {
        var dir = Path.Combine(RepoPath, dirName);
        if (!Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }
        return Path.Combine(dir, fileName);
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
