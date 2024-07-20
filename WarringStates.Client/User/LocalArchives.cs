using System.Diagnostics.CodeAnalysis;
using WarringStates.Client.Events;
using WarringStates.User;

namespace WarringStates.Client.User;

internal static class LocalArchives
{
    static ArchiveInfo[] ArchiveInfoList { get; set; } = [];

    static PlayerArchive CurrentArchive { get; set; } = new();
    //[
    //new("测试存档中文1"),
    //];
    //[];
    //[
    //new ("测试存档中文1"),
    //new ("测试存档中文20A"),
    //new ("测试存档中文300B"),
    //new ("测试存档中文4"),
    //new ("测试存档中文5"),
    //new ("测试存档中文6"),
    //new ("测试存档中文7"),
    //new ("测试存档中文8"),
    //new ("测试存档中文9"),
    //];

    public static void ReLocate(ArchiveInfo[] infoList)
    {
        ArchiveInfoList = infoList;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static int Count => ArchiveInfoList.Length;

    public static bool TryGetArchiveId(int index, [NotNullWhen(true)] out ArchiveInfo? id)
    {
        id = null;
        if (index < 0 || index >= Count)
            return false;
        id = ArchiveInfoList[index];
        return true;
    }
}
