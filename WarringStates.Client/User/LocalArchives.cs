using WarringStates.Client.Events;
using WarringStates.User;

namespace WarringStates.Client.User;

internal static class LocalArchives
{
    public static ArchiveInfoList ArchiveInfoList { get; } = [];

    static PlayerArchive CurrentArchive { get; set; } = new();

    public static void ReLocate(ArchiveInfo[] infoList)
    {
        ArchiveInfoList.Clear();
        ArchiveInfoList.AddRange(infoList);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }
}
