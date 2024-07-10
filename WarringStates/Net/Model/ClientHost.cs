using LocalUtilities.IocpNet.Common;
using LocalUtilities.IocpNet.Protocol;
using LocalUtilities.IocpNet.Transfer;
using LocalUtilities.IocpNet.Transfer.Packet;
using LocalUtilities.TypeGeneral;
using System.Net;

namespace WarringStates.Net.Model;

public class ClientHost : Host
{
    public event NetEventHandler? OnLogined;

    public event NetEventHandler? OnDisconnected;

    public event NetEventHandler<string>? OnProcessing;

    public event NetEventHandler<string[]>? OnUpdateUserList;

    ClientService HeartBeats { get; } = new(ServiceTypes.HeartBeats);

    ClientService Operator { get; } = new(ServiceTypes.Operator);

    ClientService Upload { get; } = new(ServiceTypes.Upload);

    ClientService Download { get; } = new(ServiceTypes.Download);

    public bool IsConnect => Host is not null;

    IPEndPoint? Host { get; set; } = null;

    public ClientHost()
    {
        HeartBeats.OnLog += this.HandleLog;
        HeartBeats.OnLogined += () => OnLogined?.Invoke();
        HeartBeats.OnClosed += () => OnDisconnected?.Invoke();
        Operator.OnLog += this.HandleLog;
        Operator.OnOperate += DoOperate;
        Operator.OnOperateCallback += DoOperateCallback;
        Upload.OnLog += this.HandleLog;
        Upload.OnProcessing += (speed) => OnProcessing?.Invoke(speed);
        Download.OnLog += this.HandleLog;
        Download.OnProcessing += (speed) => OnProcessing?.Invoke(speed);
    }

    public void Login(string address, int port, string name, string password)
    {
        Host = new(IPAddress.Parse(address), port);
        UserInfo = new(name, password);
        HeartBeats.Login(Host, UserInfo);
        Operator.Login(Host, UserInfo);
        Upload.Login(Host, UserInfo);
        Download.Login(Host, UserInfo);
    }

    public void Close()
    {
        Host = null;
        UserInfo = null;
        HeartBeats.Dispose();
        Operator.Dispose();
        Upload.Dispose();
        Download.Dispose();
    }

    private void DoOperate(CommandReceiver receiver)
    {
        var sender = new CommandSender(receiver.TimeStamp, (byte)CommandCode.OperateCallback, receiver.OperateCode);
        try
        {
            switch ((OperateCode)receiver.OperateCode)
            {
                case OperateCode.Message:
                    HandleMessage(receiver);
                    break;
                case OperateCode.UpdateUserList:
                    var userList = ReadU8Buffer(receiver.Data).ToArray();
                    OnUpdateUserList?.Invoke(userList);
                    break;
            }
            Operator.CallbackSuccess(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
            Operator.CallbackFailure(sender, ex);
        }
    }

    private void DoOperateCallback(CommandReceiver receiver)
    {
        try
        {
            switch ((OperateCode)receiver.OperateCode)
            {
                case OperateCode.Message:
                    break;
            }
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void SendMessage(string message, string userName)
    {
        try
        {
            if (UserInfo is null)
                throw new IocpException(ServiceCode.EmptyUserInfo);
            var count = WriteU8Buffer(message, out var data);
            var sender = new CommandSender(DateTime.Now, (byte)CommandCode.Operate, (byte)OperateCode.Message, data, 0, count)
                .AppendArgs(ServiceKey.ReceiveUser, userName)
                .AppendArgs(ServiceKey.SendUser, UserInfo.Name);
            Operator.SendCommand(sender);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void UploadFile(string dirName, string filePath)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var localPath = Upload.GetFileRepoPath(dirName, fileName);
            if (!File.Exists(localPath))
                File.Copy(filePath, localPath);
            Upload.UploadAsync(dirName, fileName);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }

    public void DownloadFile(string dirName, string filePath)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            Download.DownLoadAsync(dirName, fileName);
        }
        catch (Exception ex)
        {
            this.HandleException(ex);
        }
    }
}
