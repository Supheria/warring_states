namespace WarringStates.Net.Common;

public enum OperateCode : byte
{
    None,
    Start,
    Request,
    Continue,
    Finish,
    Callback,
    JoinArchive,
    Broadcast,
}
