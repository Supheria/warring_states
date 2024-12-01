using LocalUtilities.General;
using LocalUtilities.IocpNet;
using System.Text;
using WarringStates.Net;
using WarringStates.Net.Common;
using WarringStates.User;

namespace WarringStates.Client.Net;

public partial class ClientService : NetService
{
    AutoResetEvent LoginDone { get; } = new(false);

    public ClientService() : base(new ClientProtocol())
    {
        HandleCommands[CommandCode.HeartBeats] = ReceiveCallback;
        HandleCommands[CommandCode.Login] = HandleLogin;
        HandleCommands[CommandCode.UploadFile] = HandleUploadFile;
        HandleCommands[CommandCode.DownloadFile] = HandleDownloadFile;
        HandleCommands[CommandCode.Message] = HandleMessage;
        HandleCommands[CommandCode.Player] = HandlePlayer;
        HandleCommands[CommandCode.Archive] = HandleArchive;
        HandleCommands[CommandCode.SpanFlow] = HandleTimer;
        HandleCommands[CommandCode.Land] = HandleLand;
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
}
