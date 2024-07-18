using AltitudeMapGenerator;
using System.Text;

namespace WarringStates.Server.User;

internal class UserException(string message) : Exception(message)
{
    public static void ThrowIfNotUseable(ArchiveInfo info)
    {
        var sb = new StringBuilder();
        if (info.WorldName is "")
            sb.Append(nameof(info.WorldName)).Append(' ');
        if (info.CreateTime is 0)
            sb.Append(nameof(info.CreateTime)).Append(' ');
        if (info.LastSaveTime is 0)
            sb.Append(nameof(info.LastSaveTime)).Append(' ');
        var map = info.LoadAltitudeMap();
        if (map.OriginPoints.Count < 1 || map.Width is 0 || map.Height is 0)
            sb.Append(nameof(AltitudeMap)).Append(' ');
        if (sb.Length is 0)
            return;
        sb.Append("did not find in save file");
        throw new UserException(sb.ToString());
    }

    public static UserException ArchiveCannotFind(ArchiveInfo info)
    {
        return new($"cannot find save file: {info.WorldName}@{info.CreateTime}");
    }

    public static UserException ArchiveBroken(ArchiveInfo info, string ex)
    {
        return new($"save file of {info.WorldName}@{info.CreateTime} is broken: {ex}");
    }
}
