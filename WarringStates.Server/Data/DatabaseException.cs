using WarringStates.Net.Common;

namespace WarringStates.Server.Data;

internal class DatabaseException(ServiceCode errorCode, string? message) : Exception(message)
{
    public ServiceCode ErrorCode { get; } = errorCode;

    public DatabaseException(ServiceCode errorCode) : this(errorCode, "")
    {

    }
}
