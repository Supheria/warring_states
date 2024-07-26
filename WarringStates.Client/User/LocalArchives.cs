using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.UI;
using WarringStates.User;

namespace WarringStates.Client.User;

internal static class LocalArchives
{
    public static ArchiveInfoRoster ArchiveInfoList { get; } = [];

    public static PlayerArchive? CurrentArchive { get; private set; } = null;

    public static void ReLocate(ArchiveInfo[] infoList)
    {
        ArchiveInfoList.Clear();
        ArchiveInfoList.AddRange(infoList);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static void SetCurrentArchive(PlayerArchive archive)
    {
        CurrentArchive = archive;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.CurrentArchiveChange);
    }

    public static void StartPlayArchive(PlayerArchive archive)
    {
        CurrentArchive = archive;
        Atlas.Relocate(archive);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.StartGamePlay);
    }
}
