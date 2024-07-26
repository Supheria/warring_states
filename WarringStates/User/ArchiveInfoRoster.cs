using LocalUtilities.TypeGeneral;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.User;

public class ArchiveInfoRoster : Roster<string, ArchiveInfo>, IList<ArchiveInfo>
{
    List<ArchiveInfo> InfoList { get; } = [];

    public ArchiveInfo this[int index] { get => InfoList[index]; set => InfoList[index] = value; }

    public ArchiveInfo[] GetArchiveInfos()
    {
        return InfoList.ToArray();
    }

    public bool TryGetValue(int index, [NotNullWhen(true)] out ArchiveInfo? info)
    {
        info = null;
        if (index < 0 || index >= Count)
            return false;
        info = InfoList[index];
        return true;
    }

    public int IndexOf(ArchiveInfo item)
    {
        return InfoList.IndexOf(item);
    }

    public void Insert(int index, ArchiveInfo item)
    {
        InfoList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        InfoList.RemoveAt(index);
    }

    public new void TryAdd(ArchiveInfo item)
    {
        InfoList.Add(item);
        base.TryAdd(item);
    }

    public new void TryRemove(ArchiveInfo item)
    {
        InfoList.Remove(item);
        base.TryRemove(item);
    }

    public void AddRange(ArchiveInfo[] items)
    {
        foreach (var item in items)
        {
            InfoList.Add(item);
            base.TryAdd(item);
        }
    }

    public new void Clear()
    {
        InfoList.Clear();
        base.Clear();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return InfoList.GetEnumerator();
    }
}
