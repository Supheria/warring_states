using LocalUtilities.TypeGeneral;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.User;

namespace WarringStates.User;

public class ArchiveInfoList : Roster<string, ArchiveInfo>, IList<ArchiveInfo>
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

    public new void Add(ArchiveInfo item)
    {
        InfoList.Add(item);
        base.Add(item);
    }

    public void AddRange(ArchiveInfo[] items)
    {
        foreach (var item in items)
        {
            InfoList.Add(item);
            base.Add(item);
        }
    }

    public new void Clear()
    {
        InfoList.Clear();
        base.Clear();
    }

    public new void CopyTo(ArchiveInfo[] array, int arrayIndex)
    {
        InfoList.CopyTo(array, arrayIndex);
    }

    public IEnumerator<ArchiveInfo> GetEnumerator()
    {
        return InfoList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return InfoList.GetEnumerator();
    }
}
