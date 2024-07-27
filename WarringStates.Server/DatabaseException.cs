using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeToolKit.Convert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Net.Common;

namespace WarringStates.Server;

internal class DatabaseException(ServiceCode errorCode, string? message) : Exception(message)
{
    public ServiceCode ErrorCode { get; } = errorCode;

    public DatabaseException(ServiceCode errorCode) : this(errorCode, "")
    {

    }
}
