namespace WarringStates.Net.Common;

public enum CommandCode : byte
{
    None,
    ComposeCommand,
    CommandError,
    Login,
    Operate,
    OperateCallback,
    HeartBeats,
    DownloadFile,
    UploadFile,
    Archive,
    Message,
}
