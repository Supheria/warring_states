namespace WarringStates.Net.Common;

public enum CommandCode : byte
{
    None = 0,
    ComposeCommand = 1,
    Login = 2,
    Operate = 3,
    OperateCallback = 4,
    HeartBeats = 5,
    DownloadFile = 6,
    UploadFile = 7,
    Archive = 8,
    Message = 9,
    Player = 10,
    SpanFlow = 11,
    Land = 12,
}
