using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Text;
using WarringStates.Net.Common;
using WarringStates.User;

namespace WarringStates.Net;

public abstract class Host : INetLogger
{
    protected UserInfo? UserInfo { get; set; } = null;
    public NetEventHandler<string>? OnLog { get; set; }

    public string GetLog(string message)
    {
        return new StringBuilder()
            .Append(UserInfo?.Name)
            .Append(SignTable.Colon)
            .Append(SignTable.Space)
            .Append(message)
            .Append(SignTable.Space)
            .Append(SignTable.At)
            .Append(DateTime.Now.ToString(DateTimeFormat.Outlook))
            .ToString();
    }

    protected static int WriteU8Buffer(string str, out byte[] buffer)
    {
        buffer = Encoding.UTF8.GetBytes(str);
        return buffer.Length;
    }

    protected static string ReadU8Buffer(byte[] buffer)
    {
        return Encoding.UTF8.GetString(buffer);
    }

    protected void HandleMessage(CommandReceiver receiver)
    {
        var str = new StringBuilder()
            .Append(receiver.GetArgs(ServiceKey.SendUser))
            .Append(SignTable.Sub)
            .Append(SignTable.Greater)
            .Append(UserInfo?.Name)
            .Append(SignTable.Colon)
            .Append(SignTable.Space)
            .Append(ReadU8Buffer(receiver.Data))
            .ToString();
        OnLog?.Invoke(str);
    }
}
