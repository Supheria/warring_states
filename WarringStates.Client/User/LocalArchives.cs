using System.Diagnostics.CodeAnalysis;
using WarringStates.Client.Events;
using WarringStates.User;

namespace WarringStates.Client.User;

internal static class LocalArchives
{
    static List<PlayerArchiveInfo> Archives { get; set; } = [];
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

    public static void ReLocate(List<PlayerArchiveInfo> infos)
    {
        Archives = infos;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.RelocateArchiveList);
    }

    public static int Count => Archives.Count;

    public static bool TryGetArchiveInfo(int index, [NotNullWhen(true)] out PlayerArchiveInfo? info)
    {
        info = null;
        if (index < 0 || index >= Count)
            return false;
        info = Archives[index];
        return true;
    }
}
