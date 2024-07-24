using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.UI;
using WarringStates.User;

namespace WarringStates.Client.User;

internal static class LocalArchives
{
    public static ArchiveInfoList ArchiveInfoList { get; } = [];

    public static PlayerArchive CurrentArchive { get; set; } = new();

    public static void ReLocate(ArchiveInfo[] infoList)
    {
        ArchiveInfoList.Clear();
        ArchiveInfoList.AddRange(infoList);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }
}
