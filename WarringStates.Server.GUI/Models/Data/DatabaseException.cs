using WarringStates.Net.Common;
using System;

namespace WarringStates.Server.GUI.Models;

internal class DatabaseException(ServiceCode errorCode, string? message) : Exception(message)
{
    public ServiceCode ErrorCode { get; } = errorCode;

    public DatabaseException(ServiceCode errorCode) : this(errorCode, "")
    {

    }
}
