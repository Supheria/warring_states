using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.User;

public class UserException(string message) : Exception(message)
{
    public static UserException ArchiveCannotFind(ArchiveInfo info)
    {
        return new($"cannot find save file: {info.WorldName}@{info.CreateTime}");
    }

    public static UserException ArchiveBroken(ArchiveInfo info, string ex)
    {
        return new($"save file of {info.WorldName}@{info.CreateTime} is broken: {ex}");
    }
}
