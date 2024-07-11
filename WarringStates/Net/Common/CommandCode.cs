namespace WarringStates.Net.Common;

public enum CommandCode : byte
{
    None,
    Login,
    Operate,
    OperateCallback,
    HeartBeats,
    DownloadFile,
    UploadFile,
}
