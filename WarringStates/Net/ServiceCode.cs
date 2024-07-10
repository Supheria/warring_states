using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Net;

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
    CannotAddSendCommand,
    EmptyUserInfo,
    NoConnection,
}
