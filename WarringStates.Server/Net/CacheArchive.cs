using WarringStates.Net.Utilities;
using WarringStates.Server.Map;
using WarringStates.Server.User;
using WarringStates.User;

namespace WarringStates.Server.Net;

internal class CacheArchive(ArchiveInfo archiveInfo) : AutoDisposeItem(ConstTable.ArchiveDisposeMilliseconds)
{
    public ArchiveInfo ArchiveInfo { get; } = archiveInfo;

    public LandMapEx LandMap { get; } = LocalArchive.InitializeLandMap(archiveInfo);
}
