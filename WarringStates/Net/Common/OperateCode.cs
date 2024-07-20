namespace WarringStates.Net.Common;

public enum OperateCode : byte
{
    None = 0,
    Start = 1,
    Request = 2,
    Continue = 3,
    Finish = 4,
    Callback = 5,
    Join = 6,
    Broadcast = 7,
    List = 8,
}
