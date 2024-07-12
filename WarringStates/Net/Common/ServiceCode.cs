namespace WarringStates.Net.Common;

public enum ServiceCode
{
    None,
    Success,
    UnknowError,
    UnknownCommand,
    UnknownOperate,
    WrongCommandFormat,
    NotLogined,
    UserAlreadyLogined,
    WrongProtocolType,
    SameVersionAlreadyExist,
    CannotAddFileToProcess,
    FileExpired,
    NotSameVersion,
    FileNotExist,
    UserNotExist,
    ServerNotStartYet,
    ServerHasStarted,
    CannotFindSourceSendCommand,
    CannotAddCommandWaitingForCallback,
    CannotFindSourceCommandToCompose,
    CannotAddCommandWaitingForCompose,
    EmptyUserInfo,
    NoConnection,
}
