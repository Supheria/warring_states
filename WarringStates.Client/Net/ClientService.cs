using LocalUtilities.IocpNet;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Net;
using System.Text;
using WarringStates.Net;
using WarringStates.Net.Common;
using WarringStates.Net.Utilities;
using WarringStates.User;

namespace WarringStates.Client.Net;

public partial class ClientService : NetService
{
    AutoResetEvent LoginDone { get; } = new(false);

    protected override DaemonThread DaemonThread { get; init; }

    public ClientService() : base(new ClientProtocol())
    {
        DaemonThread = new(ConstTabel.HeartBeatsInterval, HeartBeats);
        HandleCommands[CommandCode.HeartBeats] = ReceiveCallback;
        HandleCommands[CommandCode.Login] = HandleLogin;
        HandleCommands[CommandCode.UploadFile] = HandleUploadFile;
        HandleCommands[CommandCode.DownloadFile] = HandleDownloadFile;
        HandleCommands[CommandCode.Message] = HandleMessage;
        HandleCommands[CommandCode.Player] = HandlePlayer;
        HandleCommands[CommandCode.Archive] = HandleArchive;
    }

    public override void HandleCommand(CommandReceiver receiver)
    {
        try
        {
            var commandCode = (CommandCode)receiver.CommandCode;
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
            .Append(Protocol.SocketInfo.LocalEndPoint)
            .Append(SignCollection.CloseBracket)
            .Append(SignCollection.Space)
            .Append(message)
            .Append(SignCollection.Space)
            .Append(SignCollection.At)
            .Append(DateTime.Now.ToString(DateTimeFormat.Outlook))
            .ToString();
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

    public void Login(string address, int port, string name, string password)
    {
        try
        {
            if (IsLogined)
                return;
            var host = new IPEndPoint(IPAddress.Parse(address), port);
            Player = new(name, password);
            if (!((ClientProtocol)Protocol).Connect(host))
                throw new NetException(ServiceCode.NoConnection);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Login, (byte)OperateCode.None)
                .AppendArgs(ServiceKey.UserName, Player.Name)
                .AppendArgs(ServiceKey.Password, Player.Password);
            SendCommand(sender);
            LoginDone?.WaitOne(ConstTabel.BlockkMilliseconds);
        }
        catch (Exception ex)
        {
            Dispose();
            this.HandleException(ex);
        }
    }

    private void HandleLogin(CommandReceiver receiver)
    {
        ReceiveCallback(receiver);
        IsLogined = true;
        LoginDone.Set();
        HandleLogined();
        DaemonThread.Start();
    }
}
