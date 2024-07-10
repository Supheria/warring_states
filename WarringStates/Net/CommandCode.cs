namespace WarringStates.Net;

public enum CommandCode : byte
{
     None,
     Login,
     Operate,
     OperateCallback,
     HeartBeats,
     TransferFile,
}
