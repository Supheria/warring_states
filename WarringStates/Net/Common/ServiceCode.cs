namespace WarringStates.Net.Common;

public enum ServiceCode
{
    None,
    Success,
    UnknowError,
    NotLogined,
    UserAlreadyLogined,
    WrongProtocolType,
    SameVersionAlreadyExist,
    ProcessingFile,
    FileExpired,
    NotSameVersion,
    FileNotExist,
    UserNotExist,
    ServerNotStartYet,
    ServerHasStarted,
    CannotFindSourceSendCommand,
    CannotWaitSendCommandForCallback,
    EmptyUserInfo,
    NoConnection,
}
